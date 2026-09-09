using Content.Shared.Weapons.Melee;

namespace Content.Shared.Execution;

/// <summary>
///     Verb for violently murdering cuffed creatures - with a melee weapon!
/// </summary>
public sealed partial class MeleeExecutionSystem : EntitySystem
{
    [SubscribeLocalEvent]
    private void OnExecution(Entity<MeleeWeaponComponent> tool, ref BeforeExecutionEvent args)
    {
        args.Sound = tool.Comp.HitSound;
        args.Damage = tool.Comp.Damage;
    }
}