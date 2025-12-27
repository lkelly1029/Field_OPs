using UnityEngine;
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
            if (Input.GetKeyDown(KeyCode.BackQuote)) // Tilde key
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
            GUILayout.Label($"Treasury: {_provider.TreasuryCents / 100.0f:C2}");

            GUILayout.Space(10);
            GUILayout.Label("<b>Metrics:</b>");
            
            foreach (var kvp in _provider.GetDebugMetrics())
            {
                if (kvp.Key.Contains("(D/S)"))
                {
                    DrawResourceBar(kvp.Key, kvp.Value);
                }
                else
                {
                    GUILayout.Label($"{kvp.Key}: {kvp.Value}");
                }
            }

            GUILayout.EndVertical(); // Ledger

            GUILayout.EndArea();
        }

        private void DrawResourceBar(string label, string value)
        {
            // Parse "Demand / Supply" e.g. "1000 / 5000"
            var parts = value.Split('/');
            if (parts.Length == 2 && long.TryParse(parts[0].Trim(), out long demand) && long.TryParse(parts[1].Trim(), out long supply))
            {
                float ratio = supply > 0 ? (float)demand / supply : 0f;
                ratio = Mathf.Clamp01(ratio); // Cap at 100% for the bar

                GUILayout.BeginVertical("box");
                GUILayout.Label($"{label}: {value}"); // Text label
                
                // Background bar
                var rect = GUILayoutUtility.GetRect(100, 20);
                GUI.Box(rect, "");

                // Fill bar
                var fillRect = new Rect(rect.x, rect.y, rect.width * ratio, rect.height);
                
                // Color based on ratio
                Color color = ratio > 1.0f ? Color.red : (ratio > 0.8f ? Color.yellow : Color.green); 
                // Logic: High demand/supply ratio means we are using most of our supply?
                // Actually: Demand 1000 / Supply 5000 = 0.2 (20% utilization) -> Good (Green)
                // Demand 5000 / Supply 5000 = 1.0 (100% utilization) -> Warning (Yellow)
                // Demand 6000 / Supply 5000 = 1.2 -> Crisis (Red)
                // Wait, if demand > supply, ratio > 1. But I clamped it.
                // Let's recalculate ratio for color without clamp.
                float rawRatio = supply > 0 ? (float)demand / supply : 1f;
                if (supply == 0 && demand > 0) rawRatio = 2f; // Infinite trouble

                Color originalColor = GUI.color;
                GUI.color = rawRatio > 1.0f ? Color.red : (rawRatio > 0.9f ? Color.yellow : Color.green);
                GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
                GUI.color = originalColor;

                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label($"{label}: {value}");
            }
        }
    }
}
