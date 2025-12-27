using System;

namespace Sovereign.Core.Primitives
{
    /// <summary>
    /// Represents a physical quantity of a specific resource.
    /// Values are stored as long (integers) to ensure determinism.
    /// </summary>
    public struct ResourceQuantity
    {
        public ResourceType Type { get; }
        public long Value { get; }

        public ResourceQuantity(ResourceType type, long value)
        {
            Type = type;
            Value = value;
        }

        public static ResourceQuantity Zero(ResourceType type) => new ResourceQuantity(type, 0);

        public static ResourceQuantity operator +(ResourceQuantity a, ResourceQuantity b)
        {
            if (a.Type != b.Type) throw new InvalidOperationException("Cannot add different resource types.");
            return new ResourceQuantity(a.Type, a.Value + b.Value);
        }

        public static ResourceQuantity operator -(ResourceQuantity a, ResourceQuantity b)
        {
            if (a.Type != b.Type) throw new InvalidOperationException("Cannot subtract different resource types.");
            return new ResourceQuantity(a.Type, a.Value - b.Value);
        }

        public override string ToString() => $"{Value} units of {Type}";
    }
}
