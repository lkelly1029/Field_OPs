using Sovereign.Core;
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace Sovereign.Sim
{
    public interface IProducer
    {
        Dictionary<ResourceType, long> GetProduction(TickIndex tick);
    }
}
