using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Sim.Buildings
{
    public class House : IConsumer
    {
        public ResourceQuantity[] GetDemands(TickIndex tick)
        {
            return new[]
            {
                new ResourceQuantity(ResourceType.Power, 1000), 
                new ResourceQuantity(ResourceType.Water, 100),   
                new ResourceQuantity(ResourceType.Food, 50)      
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
