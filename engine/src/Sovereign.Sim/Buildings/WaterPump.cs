using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Sim.Buildings
{
    public class WaterPump : IProducer
    {
        public ResourceQuantity[] Produce(TickIndex tick)
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Water, 1000) // 1000 Liters
            };
        }
    }
}
