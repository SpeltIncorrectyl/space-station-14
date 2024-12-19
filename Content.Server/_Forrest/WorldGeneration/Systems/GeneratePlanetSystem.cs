using System.Diagnostics;
using System.Linq;
using Content.Server._Forrest.WorldGeneration.Components;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Events;
using Content.Server.Station.Events;
using Content.Server.Station.Systems;
using Content.Server.Parallax;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Robust.Shared.Prototypes;

namespace Content.Server._Forrest.WorldGeneration.Systems;

public sealed class GeneratePlanetSystem: EntitySystem
{
    [Dependency] private readonly BiomeSystem _biomeSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationPostInitEvent>(OnStationPostInit);
    }

    private void OnStationPostInit(ref StationPostInitEvent ev)
    {
        Log.Debug("Post Station, Starting To Generate Planet");
        if (!TryComp<GeneratePlanetComponent>(ev.Station.Owner, out var comp))
        {
            Log.Debug("Station didn't have GeneratePlanetComponent");
            return;
        }

        var biome = _prototypeManager.Index(comp.Biome);

        var mapUids = new List<EntityUid>();

        foreach(var grid in ev.Station.Comp.Grids)
        {
            var mapUid = Transform(grid).MapUid;
            if (mapUid is null)
            {
                Log.Warning("Grid has no map uid");
                continue;
            }

            if (!mapUids.Contains(mapUid.Value))
                mapUids.Add(mapUid.Value);
        }

        foreach (var mapUid in mapUids)
        {
            Log.Debug("Generated planet");
            _biomeSystem.EnsurePlanet(mapUid, biome);
        }
    }
}
