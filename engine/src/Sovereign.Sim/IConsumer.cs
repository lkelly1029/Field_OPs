using Sovereign.Core;
using Sovereign.Core.Primitives;
using System.Collections.Generic;

namespace Sovereign.Sim
{
    public interface IConsumer
    {
        Dictionary<ResourceType, long> GetResourceDemands(TickIndex tick);
    }
}
