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
        args.Sound = tool.Comp.SoundEmpty;

        var from = Transform(args.User).Coordinates;
        var ev = new TakeAmmoEvent(1, [], from, args.User);
        RaiseLocalEvent(tool, ev);
        _gun.UpdateAmmoCount(tool);

        if (ev.Ammo.Count == 0)
            return;

        var damage = new DamageSpecifier();
        foreach (var (entity, shootable) in ev.Ammo)
        {
            if (entity is { } shootableEntity)
                HandleShootable(shootableEntity, shootable, ref damage);
        }

        args.Sound = tool.Comp.SoundGunshot;
        args.Damage = damage;
    }

    private void HandleShootable(EntityUid entity, IShootable shootable, ref DamageSpecifier damage)
    {
        if (shootable is HitscanAmmoComponent hitscan)
            HandleHitscan((entity, hitscan), ref damage);
        else if (shootable is CartridgeAmmoComponent cartridge)
            HandleCartridge((entity, cartridge), ref damage);
        else if (shootable is AmmoComponent)
            HandleProjectile(entity, ref damage);
    }

    private void HandleProjectile(EntityUid projectile, ref DamageSpecifier damage)
    {
        if (!TryComp<ProjectileComponent>(projectile, out var projectileComp))
            return;

        damage += projectileComp.Damage;
        Del(projectile);
    }

    private void HandleCartridge(Entity<CartridgeAmmoComponent> cartridge, ref DamageSpecifier damage)
    {
        var projectile = Spawn(cartridge.Comp.Prototype);
        HandleProjectile(projectile, ref damage);
    }

    private void HandleHitscan(Entity<HitscanAmmoComponent> hitscan, ref DamageSpecifier damage)
    {
        if (!TryComp<HitscanBasicDamageComponent>(hitscan, out var damageComp))
            return;

        damage += damageComp.Damage;
        Del(hitscan);
    }
}