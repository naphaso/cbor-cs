using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naphaso.Cbor.Types.Number {
    using DiverseWorlds.Logic.Network.Cbor.Exception;

    public class CborNumber32 : CborNumber
    {
        private uint value;

        private int sign;

        public CborNumber32(int sign, uint value)
        {
            this.sign = sign;
            this.value = value;
        }
        public override byte byteValue()
        {
            if (sign < 0)
            {
                throw new CborException("number is negate");
            }
            else if (value <= 255)
            {
                return (byte) value;
            }
            else
            {
                throw new CborException("number too long");
            }
        }

        public override short shortValue()
        {
            if (sign < 0)
            {
                if (value <= 32768)
                {
                    return (short) -(int) value;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
            else
            {
                if (value < short.MaxValue)
                {
                    return (short) value;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
        }

        public override int intValue()
        {
            if (sign < 0)
            {
                if (value < 2147483648)
                {
                    return - (int) value;
                }
                else if (value == 2147483648)
                {
                    return int.MinValue;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
            else
            {
                if (value <= int.MaxValue)
                {
                    return (int) value;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
        }

        public override long longValue()
        {
            return sign * (long)value;
        }

        public override float floatValue()
        {
            return sign * value;
        }

        public override double doubleValue()
        {
            return sign * value;
        }

        public override ushort ushortValue()
        {
            if (sign >= 0)
            {
                if (value <= ushort.MaxValue)
                {
                    return (ushort)value;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
            else
            {
                throw new CborException("number is negative");
            }
        }

        public override uint uintValue()
        {
            if (sign >= 0)
            {
                return value;
            } else {
                throw new CborException("number is negative");
            }
        }

        public override ulong ulongValue()
        {
            if (sign >= 0)
            {
                return value;
            }
            else
            {
                throw new CborException("number is negative");
            }
        }

        public override CborWriter Write(CborWriter writer)
        {
            base.Write(writer);

            writer.Write(sign, value);
            return writer;
        }

        public override string ToString()
        {
            return (sign < 0 ? "-" : "") + value;
        }

        protected bool Equals(CborNumber32 other)
        {
            return value == other.value && sign == other.sign;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CborNumber32)) return false;
            return Equals((CborNumber32) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) value*397) ^ sign;
            }
        }

        public static bool operator ==(CborNumber32 left, CborNumber32 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CborNumber32 left, CborNumber32 right)
        {
            return !Equals(left, right);
        }
    }
}
