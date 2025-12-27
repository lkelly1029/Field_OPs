using System;
using System.Collections.Generic;
using Sovereign.Core;
using Sovereign.Core.Primitives;
using Sovereign.Economy;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sovereign.Serialization
{
    public class UniverseState
    {
        public Guid Id { get; set; }
        public long CurrentTick { get; set; }
        public Guid TreasuryId { get; set; }
        public List<PlotStateDTO> Plots { get; set; } = new();
        public LedgerStateDTO Ledger { get; set; }
    }

    public class PlotStateDTO
    {
        public int X { get; set; }
        public int Y { get; set; }
        public PlotState State { get; set; }
        public double Stability { get; set; }
        public Dictionary<ResourceType, long> Storage { get; set; }
        // Building Type?
        // We need to know what Consumer/Producer is there.
        // For MVP, if state is Active, we assume a building based on... wait.
        // The Engine doesn't store "BuildingType" enum. It stores IConsumer/IProducer instances.
        // We need to add a way to identify the building type to Plot or serialize the type name.
        // Let's store the Type Name for now.
        public string BuildingType { get; set; }
    }

    public class LedgerStateDTO
    {
        public Dictionary<Guid, MoneyCents> MonetaryBalances { get; set; }
        public Dictionary<ResourceType, Dictionary<Guid, long>> ResourceBalances { get; set; }
    }

    public static class UniverseSerializer
    {
        public static string Serialize(Universe universe)
        {
            var state = new UniverseState
            {
                Id = universe.Id,
                CurrentTick = universe.CurrentTick.Value,
                TreasuryId = universe.TreasuryId,
                Ledger = new LedgerStateDTO
                {
                    MonetaryBalances = new Dictionary<Guid, MoneyCents>(universe.Ledger.MonetaryBalances),
                    ResourceBalances = new Dictionary<ResourceType, Dictionary<Guid, long>>(universe.Ledger.ResourceBalances)
                }
            };

            foreach (var plot in universe.Plots)
            {
                var dto = new PlotStateDTO
                {
                    X = plot.X,
                    Y = plot.Y,
                    State = plot.State,
                    Stability = plot.Stability,
                    Storage = new Dictionary<ResourceType, long>(plot.Storage),
                    BuildingType = plot.Consumer?.GetType().Name ?? plot.Producer?.GetType().Name
                };
                state.Plots.Add(dto);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Serialize(state, options);
        }

        public static Universe Deserialize(string json)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            var state = JsonSerializer.Deserialize<UniverseState>(json, options);

            var ledger = new Ledger();
            ledger.LoadState(state.Ledger.MonetaryBalances, state.Ledger.ResourceBalances);

            // We need a way to construct Universe with specific ID/TreasuryID/Tick?
            // Universe constructor generates new IDs.
            // We might need to add a "Load" method to Universe or a special constructor.
            // For MVP, we'll create a new Universe and patch the fields (if we can expose them or add Load method).
            // Let's add `LoadState` to Universe.
            
            var universe = new Universe(new GlobalExchange(), ledger);
            universe.LoadState(state.Id, state.CurrentTick, state.TreasuryId);
            
            // Reconstruct plots
            foreach (var pDto in state.Plots)
            {
                var plot = new Plot
                {
                    X = pDto.X,
                    Y = pDto.Y,
                    State = pDto.State,
                    Stability = pDto.Stability
                };
                foreach(var kvp in pDto.Storage) plot.Storage[kvp.Key] = kvp.Value;

                // Restore Building
                if (!string.IsNullOrEmpty(pDto.BuildingType))
                {
                    // Simple factory logic
                    if (pDto.BuildingType == nameof(House)) plot.Consumer = new House();
                    else if (pDto.BuildingType == nameof(Farm)) plot.Consumer = new Farm();
                    else if (pDto.BuildingType == nameof(WaterPump)) plot.Producer = new WaterPump();
                    else if (pDto.BuildingType == nameof(IronMine)) { var m = new IronMine(); plot.Producer = m; plot.Consumer = m; }
                    else if (pDto.BuildingType == nameof(SteelMill)) { var m = new SteelMill(); plot.Producer = m; plot.Consumer = m; }
                    else if (pDto.BuildingType == nameof(NuclearPlant)) plot.Producer = new NuclearPlant();
                }
                
                universe.AddPlot(plot);
            }

            return universe;
        }
    }
}
