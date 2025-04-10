using System.Numerics;
using Content.Shared.Squish;
using Robust.Client.GameObjects;

namespace Content.Client.Squish;

public sealed class SquishSystem: SharedSquishSystem
{

    /// <inheritdoc/>
    protected override void PerformSquishVisuals(Entity<SquishComponent> entity)
    {
        if (!TryComp<SpriteComponent>(entity.Owner, out var spriteComp))
            return;

        var elapsed = Timing.CurTime - entity.Comp.SquishStart;
        var proportion = Math.Min((elapsed / entity.Comp.SquishMagnitude).TotalSeconds, 1);

        // if proportion is [0, 0.5) then we squish
        // if proportion is [0.5, 1) then we unsquish

        float currentScale;
        if (proportion < 0.5)
        {
            currentScale = (float) (proportion * 2 * entity.Comp.SquishMagnitude);
        }
        else
        {
            currentScale = (float) (1 - (proportion - 0.5) * 2);
        }

        spriteComp.Scale = new Vector2(1 / currentScale, currentScale);
    }

    protected override void FullyUnsquish(Entity<SquishComponent> entity)
    {
        if (!TryComp<SpriteComponent>(entity.Owner, out var spriteComp))
            return;

        spriteComp.Scale = Vector2.One;
    }
}
