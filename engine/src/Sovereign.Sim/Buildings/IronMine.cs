using Sovereign.Core;
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace Sovereign.Sim.Buildings
{
    public class IronMine : IProducer, IConsumer
    {
        public Dictionary<ResourceType, long> GetResourceDemands(TickIndex tick)
        {
            return new Dictionary<ResourceType, long>
            {
                { ResourceType.Power, 2000 } // 2000 Wh to run the drills
            };
        }

        public ResourceQuantity[] GetStorageCaps()
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Power, 6000) // 3 ticks buffer
            };
        }

        public Dictionary<ResourceType, long> GetProduction(TickIndex tick)
        {
            return new Dictionary<ResourceType, long>
            {
                { ResourceType.Iron, 200 } // 200 Tonnes
            };
        }
    }
}