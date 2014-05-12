using Naphaso.Cbor.Types.Number;

namespace Naphaso.Cbor.Types {
    /// <summary>
    /// The cbor special.
    /// </summary>
    public class CborSpecial : CborObject
    {
        public static readonly CborSpecial True = new CborSpecial(21u);
        public static readonly CborSpecial False = new CborSpecial(22u);
        public CborNumber Value { get; set; }


        public CborSpecial(CborNumber value)
        {
            Value = value;
        }

        public CborSpecial(uint value)
        {
            Value = new CborNumber32(1, value);
        }


        public bool IsNull
        {
            get { return Value == 22; }
        }

        public bool IsTrue
        {
            get { return Value == 21; }
        }

        public bool IsFalse
        {
            get { return Value == 22; }
        }

        public bool IsBool
        {
            get { return IsTrue || IsFalse; }
        }

        public bool AsBool
        {
            get { return IsTrue; }
        }

        public override CborWriter Write(CborWriter writer)
        {
            base.Write(writer);

            writer.writeSpecial(Value);
            return writer;
        }

        public override string ToString()
        {
            if (IsBool)
            {
                return IsTrue ? "true" : "false";
            }
            else if (IsNull)
            {
                return "null";
            }
            else
            {
                return "special";
            }
        }

        protected bool Equals(CborSpecial other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CborSpecial)) return false;
            return Equals((CborSpecial) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(CborSpecial left, CborSpecial right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CborSpecial left, CborSpecial right)
        {
            return !Equals(left, right);
        }
    }
}
