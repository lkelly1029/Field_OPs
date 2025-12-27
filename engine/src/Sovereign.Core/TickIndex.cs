namespace Sovereign.Core.Primitives
{
    public struct TickIndex
    {
        public long Value { get; }

        public TickIndex(long value)
        {
            Value = value;
        }

        public static implicit operator long(TickIndex tick) => tick.Value;
        public static implicit operator TickIndex(long value) => new TickIndex(value);
        public static TickIndex operator ++(TickIndex tick) => new TickIndex(tick.Value + 1);
        public override string ToString() => Value.ToString();
    }
}
