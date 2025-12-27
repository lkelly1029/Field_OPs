using Sovereign.Core.Primitives;

namespace Sovereign.Sim
{
    public interface IConsumer
    {
        ResourceQuantity[] GetDemands(TickIndex tick);
        ResourceQuantity[] GetStorageCaps();
    }
}
