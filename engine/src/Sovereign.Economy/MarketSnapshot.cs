using System.Collections.Generic;
using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Economy
{
    /// <summary>
    /// Represents the state of the global market at a specific tick.
    /// Used for UI visualization and historical analysis.
    /// </summary>
    public class MarketSnapshot
    {
        public long Tick { get; set; }
        public Dictionary<ResourceType, ResourceMarketMetrics> Metrics { get; set; } = new();
    }

    public class ResourceMarketMetrics
    {
        public MoneyCents BestPrice { get; set; }
        public long TotalVolume { get; set; }
        public int OfferCount { get; set; }
        public double AiSharePct { get; set; }
        public double PlayerSharePct { get; set; }
        public List<ResourceOffer> TopOffers { get; set; } = new();
    }
}