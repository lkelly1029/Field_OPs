using Sovereign.Core;
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace Sovereign.Sim.Buildings
{
    public class House : IConsumer
    {
        public Dictionary<ResourceType, long> GetResourceDemands(TickIndex tick)
        {
            return new Dictionary<ResourceType, long>
            {
                { ResourceType.Power, 1000 },
                { ResourceType.Water, 100 },
                { ResourceType.Food, 50 }
            };
        }

        public ResourceQuantity[] GetStorageCaps()
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Power, 5000), // 5 ticks buffer
                new ResourceQuantity(ResourceType.Water, 500),  
                new ResourceQuantity(ResourceType.Food, 250)    
            };
        }
    }
}
