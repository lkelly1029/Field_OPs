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

namespace Sovereign.Sim.Serialization
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
        public Guid OwnerId { get; set; }
        public PlotState State { get; set; }
        public double Stability { get; set; }
        public Dictionary<string, long> Storage { get; set; } // Changed to string key
        public string BuildingType { get; set; }
    }

    public class LedgerStateDTO
    {
        public Dictionary<Guid, MoneyCents> MonetaryBalances { get; set; }
        public Dictionary<string, Dictionary<Guid, long>> ResourceBalances { get; set; } // Changed to string key
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
                    ResourceBalances = universe.Ledger.ResourceBalances.ToDictionary(
                        k => k.Key.ToString(), 
                        v => new Dictionary<Guid, long>(v.Value))
                }
            };

            foreach (var plot in universe.Plots)
            {
                var dto = new PlotStateDTO
                {
                    X = plot.X,
                    Y = plot.Y,
                    OwnerId = plot.OwnerId,
                    State = plot.State,
                    Stability = plot.Stability,
                    Storage = plot.Storage.ToDictionary(k => k.Key.ToString(), v => v.Value),
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
            var resourceBalances = state.Ledger.ResourceBalances.ToDictionary(
                k => Enum.Parse<ResourceType>(k.Key), 
                v => new Dictionary<Guid, long>(v.Value));
            
            ledger.LoadState(state.Ledger.MonetaryBalances, resourceBalances);

            var universe = new Universe(new GlobalExchange(), ledger);
            universe.LoadState(state.Id, state.CurrentTick, state.TreasuryId);
            
            // Reconstruct plots
            foreach (var pDto in state.Plots)
            {
                var plot = new Plot
                {
                    X = pDto.X,
                    Y = pDto.Y,
                    OwnerId = pDto.OwnerId,
                    State = pDto.State,
                    Stability = pDto.Stability
                };
                
                if (pDto.Storage != null)
                {
                    foreach(var kvp in pDto.Storage) 
                    {
                        plot.Storage[Enum.Parse<ResourceType>(kvp.Key)] = kvp.Value;
                    }
                }

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
