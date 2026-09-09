using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Content.Shared.Suicide;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Execution;

/// <summary>
///     Verb for violently murdering cuffed creatures.
/// </summary>
public sealed partial class ExecutionSystem : EntitySystem
{
    [Dependency] private ActionBlockerSystem _actionBlocker = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedSuicideSystem _suicide = default!;
    [Dependency] private DamageableSystem _damage = default!;

    /// <summary>
    /// Can this user execute this victim with this tool?
    /// If the victim can act and potentially resist then the attempt fails.
    /// This only fails on the tool side if the execution is completely implausible, like if a retracted esword is used.
    /// Executions which look plausible but don't actually kill, like shooting someone with an empty gun or a disabler, still pass this check.
    /// </summary>
    public bool CanExecute(EntityUid user, EntityUid victim, Entity<ExecutionComponent> tool)
    {
        // if the user can interact it means they are not cuffed or in crit
        // executing a guy who wasn't paying attention for a second is a no-go
        // ignore this if you are executing yourself
        if (_actionBlocker.CanInteract(victim, null) && user != victim)
            return false;

        var ev = new AttemptExecutionEvent();
        RaiseLocalEvent(tool, ref ev);
        return !ev.Cancelled;
    }

    /// <summary>
    /// Try to start an execution do-after.
    /// The <see cref="CanExecute"/> is checked before starting and also double-checked once the do-after finishes.
    /// </summary>
    public bool TryStartExecution(EntityUid user, EntityUid victim, Entity<ExecutionComponent> tool)
    {
        if (!CanExecute(user, victim, tool))
            return false;

        string loc;
        if (user == victim)
            loc = tool.Comp.BeforeSelfExecutionMessage;
        else
            loc = tool.Comp.BeforeExecutionMessage;

        _popup.PopupEntity(Loc.GetString(loc, ("attacker", user), ("victim", victim), ("tool", tool)), victim, PopupType.SmallCaution);

        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager, user, tool.Comp.ExecutionTime, new ExecutionDoAfterEvent(), tool)
        {
            Target = victim,
            Used = tool,
            BreakOnDamage = true,
            BreakOnMove = true,
            NeedHand = true
        });

        return true;
    }

    /// <summary>
    /// Try to perform an execution on a victim using a specific tool.
    /// This is the final step after the do-after.
    /// See <see cref="CanExecute"/> for when this can fail.
    /// </summary>
    public bool TryExecute(EntityUid user, EntityUid victim, Entity<ExecutionComponent> tool)
    {
        if (!CanExecute(user, victim, tool))
            return false;

        var ev = new BeforeExecutionEvent(user, victim);
        RaiseLocalEvent(tool, ref ev);

        Execute(user, victim, tool, ev.Sound, ev.Damage, ev.ForceKill);
        return true;
    }

    /// <summary>
    /// Actually does the execution once all parameters have been collected.
    /// </summary>
    private void Execute(EntityUid user, EntityUid victim, Entity<ExecutionComponent> tool, SoundSpecifier? sound, DamageSpecifier? damage, bool forceKill)
    {
        if (sound is not null)
            _audio.PlayPredicted(sound, victim, user);

        if (damage is null || damage.GetTotal() <= 0)
            return;

        if (forceKill)
        {
            if (!TryComp<DamageableComponent>(victim, out var damageable))
                return;
            _suicide.ApplyLethalDamage((victim, damageable), damage);

            string loc;
            if (user == victim)
                loc = tool.Comp.AfterSelfExecutionMessage;
            else
                loc = tool.Comp.AfterExecutionMessage;

            _popup.PopupEntity(Loc.GetString(loc, ("attacker", user), ("victim", victim), ("tool", tool)), victim, PopupType.MediumCaution);
        }
        else
        {
            _damage.ChangeDamage(victim, damage);
        }
    }

    [SubscribeLocalEvent]
    private void OnDoAfterFinished(Entity<ExecutionComponent> tool, ref ExecutionDoAfterEvent args)
    {
        if (args.Cancelled || args.Target is not { } target)
            return;

        TryExecute(args.User, target, tool);
    }

    [SubscribeLocalEvent]
    private void OnGetVerbs(Entity<ExecutionComponent> tool, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        if (!CanExecute(args.User, args.Target, tool))
            return;

        var user = args.User;
        var target = args.Target;
        args.Verbs.Add(new UtilityVerb
        {
            Act = () => TryStartExecution(user, target, tool),
            Text = Loc.GetString("execution-verb-name"),
            Message = Loc.GetString("execution-verb-message"),
        });
    }
}

/// <summary>
/// Raised on an execution tool to see if execution can even be done.
/// The only time this is cancelled it is completely impratical to even attempt an execution, like if an esword is retracted.
/// Empty guns and harmless disablers and practice rounds don't cancel this event.
/// </summary>
[ByRefEvent]
public record struct AttemptExecutionEvent(bool Cancelled = false);

/// <summary>
/// This event is raised on entities involved in the execution and the information collected determines everything about the execution.
/// </summary>
/// <param name="User">Who is doing the execution?</param>
/// <param name="Victim">Who is the victim of the execution?</param>
/// <param name="Sound">What sound will play?</param>
/// <param name="Damage">What damage is done? If the damage is null then the sound will still play but there will be no final popup. Used for 'fake' executions like if the gun is empty.</param>
/// <param name="ForceKill">By default the damage dealt will be boosted to guarantee a kill.</param>
[ByRefEvent]
public record struct BeforeExecutionEvent(
    EntityUid User,
    EntityUid Victim,
    SoundSpecifier? Sound = null,
    DamageSpecifier? Damage = null,
    bool ForceKill = true
);

/// <summary>
/// The event for the titular execution do-after.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class ExecutionDoAfterEvent : SimpleDoAfterEvent;