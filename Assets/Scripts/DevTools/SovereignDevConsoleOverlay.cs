using UnityEngine;
using UnityEngine.InputSystem;
using SovereignState.Unity.SimBridge;
using System.Collections.Generic;

namespace SovereignState.Unity.DevTools
{
    public class SovereignDevConsoleOverlay : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The GameObject that implements ISimDebugProvider")]
        public GameObject simProviderObject;

        private ISimDebugProvider _provider;
        private bool _isVisible = true;

        void Start()
        {
            if (simProviderObject != null)
            {
                _provider = simProviderObject.GetComponent<ISimDebugProvider>();
            }

            if (_provider == null)
            {
                Debug.LogWarning("SovereignDevConsoleOverlay: No ISimDebugProvider found.");
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

            GUILayout.BeginArea(new Rect(10, 10, 300, 500));
            GUILayout.BeginVertical("box");

            GUILayout.Label($"<b>Sovereign State Dev Console</b>");
            GUILayout.Label($"Tick: {_provider.CurrentTick}");
            GUILayout.Label($"💰 Treasury: {_provider.TreasuryCents / 100.0f:C2}");

            GUILayout.Space(5);
            GUILayout.Label("<b>🎮 Sim Control:</b>");
            GUILayout.BeginHorizontal();
            
            var runner = simProviderObject.GetComponent<SimulationRunner>();
            if (runner != null)
            {
                if (GUILayout.Button(runner.IsPaused ? "▶ Play" : "⏸ Pause"))
                {
                    runner.IsPaused = !runner.IsPaused;
                }
                if (GUILayout.Button("⏯ Step"))
                {
                    runner.StepTick();
                }
                if (GUILayout.Button("💾 Save"))
                {
                    runner.SaveGame();
                }
                if (GUILayout.Button("📂 Load"))
                {
                    runner.LoadGame();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Label("<b>🏗️ Building Selector:</b>");
            string[] buildings = { "House", "Farm", "WaterPump", "IronMine", "SteelMill", "NuclearPlant", "Clear" };
            
            BuildingManager manager = BuildingManager.Instance;
            if (manager != null)
            {
                // Draw buttons in a grid-like layout (2 per row for MVP)
                for (int i = 0; i < buildings.Length; i += 2)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(buildings[i])) manager.SelectedBuilding = buildings[i];
                    if (i + 1 < buildings.Length)
                    {
                        if (GUILayout.Button(buildings[i+1])) manager.SelectedBuilding = buildings[i+1];
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Label($"Selected: <color=yellow>{manager.SelectedBuilding}</color>");
            }

            GUILayout.Space(10);
            GUILayout.Label("<b>📊 Metrics:</b>");
            
            foreach (var kvp in _provider.GetDebugMetrics())
            {
                if (kvp.Key.Contains("(D/S)"))
                {
                    DrawResourceBar(kvp.Key, kvp.Value);
                }
                else if (kvp.Key.Contains("Price") || kvp.Key.Contains("Share"))
                {
                    // These are handled within the DrawResourceBar logic if needed, 
                    // or just displayed as labels here. 
                    // For now, let's keep them as labels but with some styling.
                    GUILayout.Label($"   <color=grey>{kvp.Key}: {kvp.Value}</color>");
                }
                else
                {
                    GUILayout.Label($"{kvp.Key}: {kvp.Value}");
                }
            }

            GUILayout.EndVertical(); // Ledger

            // Tooltip Area
            var tooltipPlot = TooltipManager.Instance?.HoveredPlot;
            if (tooltipPlot != null)
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical("box");
                GUILayout.Label($"<b>📍 Plot ({tooltipPlot.X}, {tooltipPlot.Y})</b>");
                GUILayout.Label($"State: {tooltipPlot.State}");
                GUILayout.Label($"Stability: {tooltipPlot.Stability:F1}%");
                
                if (tooltipPlot.Storage.Count > 0)
                {
                    GUILayout.Label("<b>Storage:</b>");
                    foreach (var kvp in tooltipPlot.Storage)
                    {
                        GUILayout.Label($"{kvp.Key}: {kvp.Value}");
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.EndArea();
        }

        private void DrawResourceBar(string label, string value)
        {
            // Resource Icon Mapping
            string icon = "";
            if (label.Contains("Power")) icon = "⚡";
            else if (label.Contains("Water")) icon = "💧";
            else if (label.Contains("Food")) icon = "🍞";
            else if (label.Contains("Steel")) icon = "🏗️";
            else if (label.Contains("Iron")) icon = "⛏️";

            // Clean label (remove suffix)
            string cleanLabel = label.Replace(" (D/S)", "");

            // Parse "Demand / Supply" e.g. "1000 / 5000"
            var parts = value.Split('/');
            if (parts.Length == 2 && long.TryParse(parts[0].Trim(), out long demand) && long.TryParse(parts[1].Trim(), out long supply))
            {
                float ratio = supply > 0 ? (float)demand / supply : 0f;
                ratio = Mathf.Clamp01(ratio); 

                GUILayout.BeginVertical("box");
                GUILayout.Label($"{icon} {cleanLabel}: <b>{demand}</b> / {supply}"); 
                
                // Background bar
                var rect = GUILayoutUtility.GetRect(100, 12);
                GUI.Box(rect, "");

                // Fill bar
                var fillRect = new Rect(rect.x + 1, rect.y + 1, (rect.width - 2) * ratio, rect.height - 2);
                
                float rawRatio = supply > 0 ? (float)demand / supply : 1f;
                if (supply == 0 && demand > 0) rawRatio = 2f; 

                Color originalColor = GUI.color;
                GUI.color = rawRatio > 1.0f ? Color.red : (rawRatio > 0.9f ? Color.yellow : Color.cyan);
                GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
                GUI.color = originalColor;

                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label($"{icon} {label}: {value}");
            }
        }
    }
}
