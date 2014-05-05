
namespace Naphaso.Cbor.Types.Number
{
    public class CborNumberDouble : CborNumber
    {
        public CborNumberDouble(double value)
        {
            Value = value;
        }

        public double Value { get; set; }
        public override byte byteValue()
        {
            return (byte) Value;
        }

        public override short shortValue()
        {
            return (short) Value;
        }

        public override int intValue()
        {
            return (int) Value;
        }

        public override long longValue()
        {
            return (long) Value;
        }

        public override float floatValue()
        {
            return (float) Value;
        }

        public override double doubleValue()
        {
            return Value;
        }

        public override ushort ushortValue()
        {
            return (ushort) Value;
        }

        public override uint uintValue()
        {
            return (uint) Value;
        }

        public override ulong ulongValue()
        {
            return (ulong) Value;
        }

        public override CborWriter Write(CborWriter writer)
        {
            base.Write(writer);

            writer.Write(Value);
            return writer;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        protected bool Equals(CborNumberDouble other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CborNumberDouble)) return false;
            return Equals((CborNumberDouble) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(CborNumberDouble left, CborNumberDouble right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CborNumberDouble left, CborNumberDouble right)
        {
            return !Equals(left, right);
        }
    }
}
