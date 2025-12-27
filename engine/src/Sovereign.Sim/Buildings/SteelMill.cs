using Sovereign.Core;
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace Sovereign.Sim.Buildings
{
    public class SteelMill : IProducer, IConsumer
    {
        public Dictionary<ResourceType, long> GetResourceDemands(TickIndex tick)
        {
            return new Dictionary<ResourceType, long>
            {
                { ResourceType.Power, 5000 },
                { ResourceType.Water, 2000 },
                { ResourceType.Iron, 150 }
            };
        }

        public ResourceQuantity[] GetStorageCaps()
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Power, 15000), // 3 ticks
                new ResourceQuantity(ResourceType.Water, 6000), 
                new ResourceQuantity(ResourceType.Iron, 450)    
            };
        }

        public Dictionary<ResourceType, long> GetProduction(TickIndex tick)
        {
            return new Dictionary<ResourceType, long>
            {
                { ResourceType.Steel, 100 }
            };
        }
    }
}
