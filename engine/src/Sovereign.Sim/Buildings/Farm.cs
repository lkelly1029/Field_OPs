using Sovereign.Core;
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace Sovereign.Sim.Buildings
{
    public class Farm : IConsumer
    {
        public Dictionary<ResourceType, long> GetResourceDemands(TickIndex tick)
        {
            return new Dictionary<ResourceType, long>
            {
                { ResourceType.Water, 500 }
            };
        }

        public ResourceQuantity[] GetStorageCaps()
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Water, 2500) // 5 ticks
            };
        }
    }
}