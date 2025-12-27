using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Sim.Buildings
{
    public class IronMine : IProducer, IConsumer
    {
        public ResourceQuantity[] GetDemands(TickIndex tick)
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Power, 2000) // 2000 Wh to run the drills
            };
        }

        public ResourceQuantity[] GetStorageCaps()
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Power, 6000) // 3 ticks buffer
            };
        }

        public ResourceQuantity[] Produce(TickIndex tick)
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Iron, 200) // 200 Tonnes
            };
        }
    }
}