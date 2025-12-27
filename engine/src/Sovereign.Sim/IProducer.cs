using Sovereign.Core.Primitives;

namespace Sovereign.Sim
{
    public interface IProducer
    {
        ResourceQuantity[] Produce(TickIndex tick);
    }
}