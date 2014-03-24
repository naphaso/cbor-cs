using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiverseWorlds.Logic.Network.Cbor.Exception;

namespace Naphaso.Cbor.Types.Number
{
    public class CborNumber64 : CborNumber
    {
        private ulong value;
        private int sign;

        public CborNumber64(int sign, ulong value)
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
            else if (value <= byte.MaxValue)
            {
                return (byte)value;
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
                    return (short)-(int)value;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
            else
            {
                if (value <= (ulong)short.MaxValue)
                {
                    return (short)value;
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
                    return -(int)value;
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
                    return (int)value;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
        }

        public override long longValue()
        {
            if (sign < 0)
            {

                if (value < 9223372036854775808UL)
                {
                    return -(long)value;
                }
                else if (value == 9223372036854775808UL)
                {
                    return long.MinValue;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
            else
            {
                if (value <= long.MaxValue)
                {
                    return (long)value;
                }
                else
                {
                    throw new CborException("number too long");
                }
            }
        }

        public override float floatValue()
        {
            return sign * (float)value;
        }

        public override double doubleValue()
        {
            return sign*(double) value;
        }

        public override ushort ushortValue()
        {
            if (sign >= 0)
            {
                if (value <= ushort.MaxValue)
                {
                    return (ushort) value;
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
                if (value <= uint.MaxValue)
                {
                    return (uint)value;
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

        protected bool Equals(CborNumber64 other)
        {
            return value == other.value && sign == other.sign;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CborNumber64) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (value.GetHashCode()*397) ^ sign;
            }
        }

        public static bool operator ==(CborNumber64 left, CborNumber64 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CborNumber64 left, CborNumber64 right)
        {
            return !Equals(left, right);
        }
    }
}
