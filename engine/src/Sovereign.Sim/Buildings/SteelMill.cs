using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Sim.Buildings
{
    public class SteelMill : IProducer, IConsumer
    {
        public ResourceQuantity[] GetDemands(TickIndex tick)
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Power, 5000), 
                new ResourceQuantity(ResourceType.Water, 2000), 
                new ResourceQuantity(ResourceType.Iron, 150)    
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

        public ResourceQuantity[] Produce(TickIndex tick)
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Steel, 100) 
            };
        }
    }
}
