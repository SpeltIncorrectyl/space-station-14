using Content.Shared.Parallax.Biomes;
using Robust.Shared.Prototypes;

namespace Content.Server._Forrest.WorldGeneration.Components;

[RegisterComponent]
public sealed partial class GeneratePlanetComponent: Component
{
    [DataField]
    public ProtoId<BiomeTemplatePrototype> Biome;
}
