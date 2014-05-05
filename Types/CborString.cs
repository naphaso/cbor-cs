

namespace Naphaso.Cbor.Types {
    public class CborString : CborObject {
        private string Value { get; set; }

        public static implicit operator string(CborString obj)
        {
            return obj.Value;
        }

        public CborString(string value)
        {
            this.Value = value;
        }

        public override bool IsString
        {
            get
            {
                return true;
            }
        }

        public override CborWriter Write(CborWriter writer)
        {
            base.Write(writer);
            writer.Write(Value);
            return writer;
        }

        public override CborString AsString
        {
            get
            {
                return this;
            }
        }

        public override string ToString()
        {
            return string.Format("\"{0}\"", Value);
        }

        protected bool Equals(CborString other)
        {
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CborString)) return false;
            return Equals((CborString) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(CborString left, CborString right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CborString left, CborString right)
        {
            return !Equals(left, right);
        }
    }
}
