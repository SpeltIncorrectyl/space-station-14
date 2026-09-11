using Content.Shared.Damage;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Hitscan.Components;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;

namespace Content.Shared.Execution;

/// <summary>
///     Verb for violently murdering cuffed creatures - with a melee weapon!
/// </summary>
public sealed partial class GunExecutionSystem : EntitySystem
{
    [Dependency] private SharedGunSystem _gun = default!;

    [SubscribeLocalEvent]
    private void OnExecution(Entity<GunComponent> tool, ref BeforeExecutionEvent args)
    {
        if (args.Handled)
            return;

        args.Sound = tool.Comp.SoundEmpty;
        args.Handled = true;

        var from = Transform(args.User).Coordinates;
        var ev = new TakeAmmoEvent(1, [], from, args.User);
        RaiseLocalEvent(tool, ev);
        _gun.UpdateAmmoCount(tool);

        if (ev.Ammo.Count == 0)
            return;

        foreach (var (entity, _) in ev.Ammo)
        {
            if (entity is { } shootableEntity)
                RaiseLocalEvent(shootableEntity, ref args);
        }

        args.Sound = tool.Comp.SoundGunshot;
    }

    [SubscribeLocalEvent]
    private void OnExecution(Entity<AmmoComponent> projectile, ref BeforeExecutionEvent args)
    {
        if (!TryComp<ProjectileComponent>(projectile, out var projectileComp))
            return;

        args.Damage += projectileComp.Damage;
        Del(projectile);
    }

    [SubscribeLocalEvent]
    private void OnExecution(Entity<CartridgeAmmoComponent> cartridge, ref BeforeExecutionEvent args)
    {
        var projectile = Spawn(cartridge.Comp.Prototype);
        RaiseLocalEvent(projectile, ref args);
        _gun.SetCartridgeSpent(cartridge, cartridge.Comp, true);
    }

    [SubscribeLocalEvent]
    private void OnExecution(Entity<HitscanAmmoComponent> hitscan, ref BeforeExecutionEvent args)
    {
        if (!TryComp<HitscanBasicDamageComponent>(hitscan, out var damageComp))
            return;

        args.Damage += damageComp.Damage;
        Del(hitscan);
    }
}