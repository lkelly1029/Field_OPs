using Sovereign.Core;
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace Sovereign.Sim.Buildings
{
    public class NuclearPlant : IProducer
    {
        public Dictionary<ResourceType, long> GetProduction(TickIndex tick)
        {
            return new Dictionary<ResourceType, long>
            {
                { ResourceType.Power, 50000 } // 50 MWh
            };
        }
    }
}