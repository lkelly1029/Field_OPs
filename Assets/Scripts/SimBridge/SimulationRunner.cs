using UnityEngine;
using Sovereign.Sim;
using Sovereign.Economy;
using Sovereign.Core; 
using Sovereign.Core.Primitives;
using Sovereign.Sim.Serialization;
using System.Collections.Generic;
using System.IO;

namespace SovereignState.Unity.SimBridge
{
    /// <summary>
    /// Connects the headless Sovereign Engine to the Unity scene.
    /// Runs the Simulation Loop in FixedUpdate.
    /// </summary>
    public class SimulationRunner : MonoBehaviour, ISimDebugProvider
    {
        private Universe _universe;
        public bool IsPaused { get; set; } = false;

        // --- ISimDebugProvider Implementation ---

        public long CurrentTick => _universe?.CurrentTick.Value ?? 0;

        public long TreasuryCents => _universe?.Ledger?.GetBalance(_universe.TreasuryId).Value ?? 0;

        public Universe GetUniverse() => _universe;

        public Dictionary<string, string> GetDebugMetrics()
        {
            var metrics = new Dictionary<string, string>();
            if (_universe != null)
            {
                metrics["Phase"] = IsPaused ? "Paused" : "Running";
                metrics["Treasury ID"] = _universe.TreasuryId.ToString();
                metrics["Net Change"] = $"{_universe.NetTreasuryChangeLastTick:+#;-#;0} cents";
                
                // Resource Metrics
                foreach (var type in new[] { ResourceType.Power, ResourceType.Water, ResourceType.Food, ResourceType.Steel, ResourceType.Iron })
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
            if (_universe != null && !IsPaused)
            {
                _universe.Tick();
            }
        }

        public void StepTick()
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

        public void SaveGame(string filename = "savegame.json")
        {
            if (_universe == null) return;
            string json = UniverseSerializer.Serialize(_universe);
            string path = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllText(path, json);
            Debug.Log($"Game saved to {path}");
        }

        public void LoadGame(string filename = "savegame.json")
        {
            string path = Path.Combine(Application.persistentDataPath, filename);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                _universe = UniverseSerializer.Deserialize(json);
                Debug.Log($"Game loaded from {path}");
            }
            else
            {
                Debug.LogWarning($"Save file not found at {path}");
            }
        }
    }
}
