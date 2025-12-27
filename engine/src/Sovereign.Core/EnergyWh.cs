namespace Sovereign.Core.Primitives
{
    public struct EnergyWh
    {
        public long Value { get; }

        public EnergyWh(long value)
        {
            Value = value;
        }

        public static implicit operator long(EnergyWh energy) => energy.Value;
        public static implicit operator EnergyWh(long value) => new EnergyWh(value);
        public static EnergyWh operator +(EnergyWh a, EnergyWh b) => new EnergyWh(a.Value + b.Value);
        public override string ToString() => $"{Value} Wh";
    }
}
