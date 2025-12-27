using UnityEngine;
using Sovereign.Sim;

namespace SovereignState.Unity.SimBridge
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance { get; private set; }
        public Plot HoveredPlot { get; set; }

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
    }
}
