using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Sim.Buildings
{
    public class Farm : IConsumer
    {
        public ResourceQuantity[] GetDemands(TickIndex tick)
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Water, 500) 
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