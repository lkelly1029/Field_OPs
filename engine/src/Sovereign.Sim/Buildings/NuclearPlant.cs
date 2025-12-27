using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Sim.Buildings
{
    public class NuclearPlant : IProducer
    {
        public ResourceQuantity[] Produce(TickIndex tick)
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Power, 50000) // 50 MWh
            };
        }
    }
}