using UnityEngine;
using Sovereign.Sim;
using Sovereign.Economy;
using Sovereign.Core; 
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace SovereignState.Unity.SimBridge
{
    /// <summary>
    /// Connects the headless Sovereign Engine to the Unity scene.
    /// Runs the Simulation Loop in FixedUpdate.
    /// </summary>
    public class SimulationRunner : MonoBehaviour, ISimDebugProvider
    {
        private Universe _universe;

        // --- ISimDebugProvider Implementation ---

        public long CurrentTick => _universe?.CurrentTick.Value ?? 0;

        public long TreasuryCents => _universe?.Ledger?.GetBalance(_universe.TreasuryId).Value ?? 0;

        public Dictionary<string, string> GetDebugMetrics()
        {
            var metrics = new Dictionary<string, string>();
            if (_universe != null)
            {
                metrics["Phase"] = "Running";
                metrics["Treasury ID"] = _universe.TreasuryId.ToString();
                metrics["Net Change"] = $"{_universe.NetTreasuryChangeLastTick:+#;-#;0} cents";
                
                // Resource Metrics
                foreach (var type in new[] { ResourceType.Power, ResourceType.Water })
                {
                    long demand = _universe.TotalDemandLastTick.ContainsKey(type) ? _universe.TotalDemandLastTick[type] : 0;
                    long supply = _universe.TotalSupplyLastTick.ContainsKey(type) ? _universe.TotalSupplyLastTick[type] : 0;
                    metrics[$"{type} (D/S)"] = $"{demand} / {supply}";
                }
            }
            else
            {
                metrics["Phase"] = "Not Initialized";
            }
            return metrics;
        }

        // --- Unity Lifecycle ---

        void Awake()
        {
            InitializeEngine();
        }

        void FixedUpdate()
        {
            if (_universe != null)
            {
                _universe.Tick();
            }
        }

        private void InitializeEngine()
        {
            // Initialize the Sovereign Engine Universe
            _universe = new Universe();
            Debug.Log($"[SimulationRunner] Universe Initialized. Treasury: {TreasuryCents}");
        }
    }
}
