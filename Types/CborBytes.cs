using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naphaso.Cbor.Types {
    public class CborBytes : CborObject {
        private byte[] Value { get; set; }

        public CborBytes(byte[] value)
        {
            this.Value = value;
        }

        public override CborBytes AsBytes
        {
            get
            {
                return this;
            }
        }

        public override bool IsBytes
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

        public override string ToString()
        {
            return string.Format("hex:{0}", BitConverter.ToString(Value).Replace("-", "").ToLower());
        }

        protected bool Equals(CborBytes other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CborBytes)) return false;
            return Equals((CborBytes) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(CborBytes left, CborBytes right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CborBytes left, CborBytes right)
        {
            return !Equals(left, right);
        }
    }
}
