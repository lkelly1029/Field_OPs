namespace Sovereign.Core.Primitives
{
    public struct MoneyCents
    {
        public long Value { get; }

        public MoneyCents(long value)
        {
            Value = value;
        }

        public static implicit operator long(MoneyCents money) => money.Value;
        public static implicit operator MoneyCents(long value) => new MoneyCents(value);
        public static MoneyCents operator -(MoneyCents a, MoneyCents b) => new MoneyCents(a.Value - b.Value);
        public static bool operator <(MoneyCents a, MoneyCents b) => a.Value < b.Value;
        public static bool operator >(MoneyCents a, MoneyCents b) => a.Value > b.Value;
        public override string ToString() => $"{Value / 100.0:C}";
    }
}
