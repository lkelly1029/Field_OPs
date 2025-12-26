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
                GUILayout.Label($"{kvp.Key}: {kvp.Value}");
            }

            GUILayout.EndVertical(); // Ledger

            GUILayout.EndArea();
        }
    }
}
