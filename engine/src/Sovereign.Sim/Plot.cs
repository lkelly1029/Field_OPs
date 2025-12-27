using System;
using System.Collections.Generic;
using System.Linq;
using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Sim
{
    public enum PlotState
    {
        Empty,
        Ghost,
        Construction,
        Built,
        Active,
        Slum,
        Abandoned
    }

    public class Plot
    {
        public int X { get; set; }
        public int Y { get; set; }
        public PlotState State { get; set; } = PlotState.Empty;
        public IConsumer Consumer { get; set; }
        public IProducer Producer { get; set; }

        public double Stability { get; set; } = 100.0;
        
        public Dictionary<ResourceType, long> Demands { get; } = new();
        public Dictionary<ResourceType, long> Deliveries { get; } = new();
        public Dictionary<ResourceType, long> Storage { get; } = new();

        public bool InputsSatisfied { get; private set; } = true;

        private int _ticksInShortage;

        private const double StabilityDecayRate = 5.0;
        private const double StabilityRecoveryRate = 2.0;
        private const int TicksToBecomeAbandoned = 50;


        public void OnTick()
        {
            if (State != PlotState.Active && State != PlotState.Slum)
            {
                return;
            }

            if (Consumer != null)
            {
                // 1. Resolve Storage Caps
                var caps = Consumer.GetStorageCaps();
                var capMap = caps.ToDictionary(c => c.Type, c => c.Value);

                // 2. Apply Decay to Storage
                var types = Storage.Keys.ToList();
                foreach (var type in types)
                {
                    double rate = ResourceDefinition.GetDecayRate(type);
                    if (rate > 0)
                    {
                        Storage[type] = (long)(Storage[type] * (1.0 - rate));
                    }
                }

                bool hasShortage = false;

                // 3. Consume Demands from Storage
                foreach (var kvp in Demands)
                {
                    ResourceType type = kvp.Key;
                    long grossDemand = kvp.Value;
                    long inStorage = Storage.ContainsKey(type) ? Storage[type] : 0;

                    if (inStorage < grossDemand)
                    {
                        hasShortage = true;
                        Storage[type] = 0; // Consume whatever is there
                    }
                    else
                    {
                        Storage[type] = inStorage - grossDemand;
                    }
                }

                if (hasShortage)
                {
                    _ticksInShortage++;
                    Stability = Math.Max(0, Stability - StabilityDecayRate);
                    InputsSatisfied = false;
                }
                else
                {
                    _ticksInShortage = 0;
                    Stability = Math.Min(100, Stability + StabilityRecoveryRate);
                    InputsSatisfied = true;
                }
            }
            else
            {
                _ticksInShortage = 0;
                InputsSatisfied = true;
            }

            UpdateStateBasedOnStability();

            // Reset for next tick (Storage persists)
            Demands.Clear();
            Deliveries.Clear();
        }

        private void UpdateStateBasedOnStability()
        {
            if (State == PlotState.Active && Stability <= 0)
            {
                State = PlotState.Slum;
                _ticksInShortage = 0;
            }
            else if (State == PlotState.Slum && _ticksInShortage >= TicksToBecomeAbandoned)
            {
                State = PlotState.Abandoned;
                Consumer = null;
            }
        }
    }
}