using Robust.Shared.Timing;

namespace Content.Shared.Squish;

public class SharedSquishSystem: EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;

    /// <summary>
    /// Apply a squish animation to an entity.
    /// </summary>
    /// <param name="uid">The entity targeted.</param>
    /// <param name="duration">How long it lasts.</param>
    /// <param name="magnitude">Magnitude of the squish as a percentage of maximum deformation.</param>
    public void Squish(EntityUid uid, TimeSpan duration, double magnitude)
    {
        Log.Debug("squished");
        var comp = new SquishComponent {SquishStart = Timing.CurTime, SquishDuration = duration, SquishMagnitude = magnitude};
        AddComp(uid, comp, true);
    }

    public void Squish(EntityUid uid, double duration = 0.2, double magnitude = 0.75)
    {
        Squish(uid, TimeSpan.FromSeconds(duration), magnitude);
    }

    /// <summary>
    /// Client side method which scales the SpriteComponent to follow the animation outlined by the SquishComponent.
    /// </summary>
    /// <param name="entity">The entity to process.</param>
    protected virtual void PerformSquishVisuals(Entity<SquishComponent> entity) {}

    /// <summary>
    /// Fully unsquish an entity just before removing the SquishComponent.
    /// We do this to ensure that the entity is fully unsquished.
    /// There is the risk they have a tiny bit of residual squishing from rounding or something.
    /// </summary>
    /// <param name="entity"></param>
    protected virtual void FullyUnsquish(Entity<SquishComponent> entity) {}

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<SquishComponent>();
        while (query.MoveNext(out var uid, out var squishComp))
        {
            PerformSquishVisuals((uid, squishComp));

            if (Timing.CurTime > squishComp.SquishStart + squishComp.SquishDuration)
            {
                Log.Debug("unsquished");
                FullyUnsquish((uid, squishComp));
                RemComp<SquishComponent>(uid);
            }
        }
    }
}
