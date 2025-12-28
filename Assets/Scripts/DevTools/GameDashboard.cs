using UnityEngine;
using UnityEngine.InputSystem;
using SovereignState.Unity.SimBridge;
using Sovereign.Sim; // For Plot
using System.Collections.Generic;
using System.Linq;

namespace SovereignState.Unity.DevTools
{
    public class GameDashboard : MonoBehaviour
    {
        [Header("References")]
        public GameObject simProviderObject;

        private ISimDebugProvider _provider;
        private SimulationRunner _runner;
        private bool _isVisible = true;

        private enum Tab { None, Build, Economy, Government, Assets }
        private Tab _currentTab = Tab.Build;

        // Assets View State
        private Vector2 _assetsScrollPos;

        void Start()
        {
            if (simProviderObject != null)
            {
                _provider = simProviderObject.GetComponent<ISimDebugProvider>();
                _runner = simProviderObject.GetComponent<SimulationRunner>();
            }
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.backquoteKey.wasPressedThisFrame) 
            {
                _isVisible = !_isVisible;
            }
        }

        void OnGUI()
        {
            if (!_isVisible || _provider == null) return;

            // 1. Sidebar Navigation
            float sidebarWidth = 80;
            float windowHeight = 600;
            
            GUILayout.BeginArea(new Rect(10, 10, sidebarWidth, windowHeight));
            GUILayout.BeginVertical("box");
            if (GUILayout.Button("🏗️\nBuild", GUILayout.Height(50))) _currentTab = Tab.Build;
            if (GUILayout.Button("💰\nEcon", GUILayout.Height(50))) _currentTab = Tab.Economy;
            if (GUILayout.Button("🏛️\nGov", GUILayout.Height(50))) _currentTab = Tab.Government;
            if (GUILayout.Button("🏭\nAssets", GUILayout.Height(50))) _currentTab = Tab.Assets;
            
            GUILayout.FlexibleSpace();
            if (_runner != null)
            {
                if (GUILayout.Button(_runner.IsPaused ? "▶" : "⏸")) _runner.IsPaused = !_runner.IsPaused;
            }
            GUILayout.Label($"Tick: {_provider.CurrentTick}");
            GUILayout.EndVertical();
            GUILayout.EndArea();

            // 2. Main Content Area
            GUILayout.BeginArea(new Rect(10 + sidebarWidth + 5, 10, 300, windowHeight));
            GUILayout.BeginVertical("box");

            switch (_currentTab)
            {
                case Tab.Build: DrawBuildTab(); break;
                case Tab.Economy: DrawEconomyTab(); break;
                case Tab.Government: DrawGovernmentTab(); break;
                case Tab.Assets: DrawAssetsTab(); break;
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();

            // 3. Asset Detail Card (Always show if something selected)
            DrawAssetCard();
        }

        void DrawBuildTab()
        {
            GUILayout.Label("<b>🏗️ Construction</b>");
            
            if (_runner != null)
            {
                if (GUILayout.Button("💾 Save Game")) _runner.SaveGame();
                if (GUILayout.Button("📂 Load Game")) _runner.LoadGame();
            }
            
            GUILayout.Space(10);
            
            string[] buildings = { "House", "Farm", "WaterPump", "IronMine", "SteelMill", "NuclearPlant", "Clear" };
            BuildingManager manager = BuildingManager.Instance;
            
            if (manager != null)
            {
                foreach (var b in buildings)
                {
                    if (GUILayout.Button(b == manager.SelectedBuilding ? $"<b>{b}</b>" : b))
                        manager.SelectedBuilding = b;
                }
            }
        }

        void DrawEconomyTab()
        {
            GUILayout.Label("<b>💰 Economy Dashboard</b>");
            GUILayout.Label($"Treasury: {_provider.TreasuryCents / 100.0f:C2}");
            if (_runner?.GetUniverse() != null)
            {
                GUILayout.Label($"Net Change: {_runner.GetUniverse().NetTreasuryChangeLastTick / 100.0f:C2}");
            }

            GUILayout.Space(10);
            GUILayout.Label("<b>Market Prices (AI):</b>");
            var uni = _runner?.GetUniverse();
            if (uni != null)
            {
                var snapshot = uni.GetMarketSnapshot();
                foreach(var kvp in snapshot.Metrics)
                {
                    GUILayout.Label($"{kvp.Key}: {kvp.Value.BestPrice.Value}¢");
                }
            }
        }

        void DrawGovernmentTab()
        {
            GUILayout.Label("<b>🏛️ Government Policy</b>");
            var gov = _runner?.GetUniverse()?.ActiveGovernment;
            if (gov != null)
            {
                GUILayout.Label($"UBI: {gov.UniversalBasicIncomeCents / 100.0f:C2}");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("- $5")) _runner.UpdateGovernment(ubi: System.Math.Max(0, gov.UniversalBasicIncomeCents - 500));
                if (GUILayout.Button("+ $5")) _runner.UpdateGovernment(ubi: gov.UniversalBasicIncomeCents + 500);
                GUILayout.EndHorizontal();

                GUILayout.Label($"Corp Tax: {gov.CorporateTaxRate:P0}");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("- 5%")) _runner.UpdateGovernment(corpTax: System.Math.Max(0, gov.CorporateTaxRate - 0.05));
                if (GUILayout.Button("+ 5%")) _runner.UpdateGovernment(corpTax: System.Math.Min(1.0, gov.CorporateTaxRate + 0.05));
                GUILayout.EndHorizontal();
            }
        }

        void DrawAssetsTab()
        {
            GUILayout.Label("<b>🏭 Asset Registry</b>");
            var uni = _runner?.GetUniverse();
            if (uni == null) return;

            // Filter active buildings
            var activePlots = uni.Plots.Where(p => p.State == PlotState.Active || p.State == PlotState.Slum).ToList();
            GUILayout.Label($"Total Assets: {activePlots.Count}");

            _assetsScrollPos = GUILayout.BeginScrollView(_assetsScrollPos);
            foreach (var plot in activePlots)
            {
                string name = plot.Producer?.GetType().Name ?? plot.Consumer?.GetType().Name ?? "Unknown";
                string status = plot.State == PlotState.Slum ? "⚠️" : "✅";
                
                if (GUILayout.Button($"{status} {name} ({plot.X},{plot.Y})"))
                {
                    // Select logic
                    if (TooltipManager.Instance != null) TooltipManager.Instance.HoveredPlot = plot;
                }
            }
            GUILayout.EndScrollView();
        }

        void DrawAssetCard()
        {
            var plot = TooltipManager.Instance?.HoveredPlot;
            if (plot == null) return;

            // Draw floating card on right
            GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, 300), "box");
            GUILayout.Label($"<b>Asset Detail</b>");
            GUILayout.Label($"Type: {plot.Producer?.GetType().Name ?? plot.Consumer?.GetType().Name ?? "Empty"}");
            GUILayout.Label($"Pos: {plot.X}, {plot.Y}");
            GUILayout.Label($"State: {plot.State}");
            GUILayout.Label($"Stability: {plot.Stability:F1}%");
            
            GUILayout.Space(5);
            GUILayout.Label("<b>Financials (Last Tick):</b>");
            GUILayout.Label($"Income: {plot.LastTickIncome / 100.0f:C2}");
            GUILayout.Label($"Tax Paid: {plot.LastTickTaxPaid / 100.0f:C2}");
            
            if (plot.Storage.Count > 0)
            {
                GUILayout.Space(5);
                GUILayout.Label("<b>Storage:</b>");
                foreach (var kvp in plot.Storage)
                {
                    GUILayout.Label($"{kvp.Key}: {kvp.Value}");
                }
            }
            GUILayout.EndArea();
        }
    }
}