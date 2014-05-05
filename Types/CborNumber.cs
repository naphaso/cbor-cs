
using Naphaso.Cbor.Types.Number;

namespace Naphaso.Cbor.Types {
    using System.Net;

    public abstract class CborNumber : CborObject {

        public override bool IsNumber
        {
            get
            {
                return true;
            }
        }

        public override CborNumber AsNumber
        {
            get
            {
                return this;
            }
        }

        public abstract byte byteValue();

        public abstract short shortValue();

        public abstract int intValue();

        public abstract long longValue();

        public abstract float floatValue();

        public abstract double doubleValue();

        public abstract ushort ushortValue();

        public abstract uint uintValue();

        public abstract ulong ulongValue();

        public static implicit operator byte(CborNumber number)
        {
            return number.byteValue();
        }

        public static implicit operator short(CborNumber number)
        {
            return number.shortValue();
        }

        public static implicit operator int(CborNumber number)
        {
            return number.intValue();
        }

        public static implicit operator long(CborNumber number)
        {
            return number.longValue();
        }

        public static implicit operator float(CborNumber number)
        {
            return number.floatValue();
        }

        public static implicit operator double(CborNumber number)
        {
            return number.doubleValue();
        }

        public static implicit operator ushort(CborNumber number)
        {
            return number.ushortValue();
        }

        public static implicit operator uint(CborNumber number)
        {
            return number.uintValue();
        }

        public static implicit operator ulong(CborNumber number)
        {
            return number.ulongValue();
        }

        public static implicit operator CborNumber(int value)
        {
            if (value < 0)
            {
                return new CborNumber32(-1, (uint)-value);
            }
            else
            {
                return new CborNumber32(1, (uint)value);
            }
        }

        public static implicit operator CborNumber(long value)
        {
            if (value < 0)
            {
                return new CborNumber64(-1, (ulong)value);
            }
            else
            {
                return new CborNumber64(1, (ulong)value);
            }
        }

        public static implicit operator CborNumber(uint value)
        {
            return new CborNumber32(1, value);
        }

        public static implicit operator CborNumber(ulong value)
        {
            return new CborNumber64(1, value);
        }

        public static implicit operator CborNumber(double value)
        {
            return new CborNumberDouble(value);
        }

    }
}
