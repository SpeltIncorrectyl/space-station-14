using Content.Server.GameTicking.Rules.Components;
using Content.Server.Parallax;
using Content.Shared.GameTicking.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules;

public sealed class PlanetBoundRuleSystem: GameRuleSystem<PlanetBoundRuleComponent>
{
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly BiomeSystem _biome = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    protected override void Started(EntityUid uid, PlanetBoundRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);
        var map = _mapSystem.GetMap(new MapId(1));
        var biome = _prototypeManager.Index(component.Biome);
        _biome.EnsurePlanet(map, biome);
    }
}
