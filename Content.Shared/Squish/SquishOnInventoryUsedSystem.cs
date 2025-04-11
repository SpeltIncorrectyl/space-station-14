using Robust.Shared.Containers;

namespace Content.Shared.Squish;

/// <summary>
/// Animates a squish effect when containers with SquishOnInventoryUsedComponent are used.
/// </summary>
public sealed class SquishOnInventoryUsedSystem: EntitySystem
{
    [Dependency] private readonly SharedSquishSystem _squish = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SquishOnInventoryUsedComponent, EntInsertedIntoContainerMessage>(OnEntInserted);
    }

    private void OnEntInserted(EntityUid entity, SquishOnInventoryUsedComponent comp, EntInsertedIntoContainerMessage args)
    {
        _squish.Squish(entity, comp.Duration, comp.Magnitude);
    }
}
