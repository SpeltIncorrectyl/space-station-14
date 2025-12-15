using Content.Shared.Parallax.Biomes;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.Components;

[RegisterComponent, Access(typeof(PlanetBoundRuleSystem))]
public sealed partial class PlanetBoundRuleComponent: Component
{
    [DataField]
    public ProtoId<BiomeTemplatePrototype> Biome;
}
