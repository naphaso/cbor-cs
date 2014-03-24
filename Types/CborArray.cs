using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naphaso.Cbor.Types {
    public class CborArray : CborObject {
        public CborObject[] Value { get; set; }

        public CborObject this[int index]
        {
            get { return Value[index]; }
            set { Value[index] = value; }
        }

        public int Count
        {
            get { return Value.Length; }
        }

        public CborArray(int size)
        {
            this.Value = new CborObject[size];
        }

        public CborArray(CborObject[] value)
        {
            this.Value = value;
        }

        public override bool IsArray
        {
            get
            {
                return true;
            }
        }

        public override CborArray AsArray
        {
            get
            {
                return this;
            }
        }

        public override CborWriter Write(CborWriter writer)
        {
            base.Write(writer);

            writer.WriteArray(Value.Count());
            foreach (var cborObject in Value)
            {
                cborObject.Write(writer);
            }
            return writer;
        }

        public override string ToString()
        {
            return string.Format("[{0}]", String.Join(", ", Value.Select((entry) => entry.ToString())));
        }

        protected bool Equals(CborArray other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CborArray)) return false;
            return Equals((CborArray) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(CborArray left, CborArray right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CborArray left, CborArray right)
        {
            return !Equals(left, right);
        }
    }
}
