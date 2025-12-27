using UnityEngine;
using Sovereign.Sim;

namespace SovereignState.Unity.SimBridge
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        public string SelectedBuilding { get; set; } = "None";

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
    }
}
