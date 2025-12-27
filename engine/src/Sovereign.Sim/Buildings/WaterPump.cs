using Sovereign.Core;
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace Sovereign.Sim.Buildings
{
    public class WaterPump : IProducer
    {
        public Dictionary<ResourceType, long> GetProduction(TickIndex tick)
        {
            return new Dictionary<ResourceType, long>
            {
                { ResourceType.Water, 1000 } // 1000 Liters
            };
        }
    }
}
