using System.Collections.Generic;

namespace SovereignState.Unity.SimBridge
{
    /// <summary>
    /// Interface for providing debug data to the on-screen developer console.
    /// This should be implemented by the main SimulationRunner MonoBehaviour.
    /// </summary>
    public interface ISimDebugProvider
    {
        long CurrentTick { get; }
        long TreasuryCents { get; }
        
        // Key-value pairs for generic debug data (e.g. "Power Load": "500MW")
        Dictionary<string, string> GetDebugMetrics();
    }
}
