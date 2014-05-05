using System;
using System.Collections.Generic;
using System.Linq;

namespace Naphaso.Cbor.Types {
    public class CborMap : CborObject
    {
        public Dictionary<CborObject, CborObject> Value { get; set; }

        public CborMap()
        {
            this.Value = new Dictionary<CborObject, CborObject>();
        }

        public void Add(CborObject key, CborObject value)
        {
            this.Value.Add(key, value);
        }

        public CborMap(Dictionary<CborObject, CborObject> map)
        {
            this.Value = map;
        }

        public override bool IsMap
        {
            get
            {
                return true;
            }
        }

        public override CborMap AsMap
        {
            get
            {
                return this;
            }
        }

        public CborObject this[CborObject key]
        {
            get { return this.Value[key]; }
            set { this.Value[key] = value; }
        }

        public override CborWriter Write(CborWriter writer)
        {
            base.Write(writer);
            writer.WriteMap(Value.Count);
            foreach (var entry in Value)
            {
                entry.Key.Write(writer);
                entry.Value.Write(writer);
            }
            return writer;
        }

        public override string ToString()
        {
            return string.Format("{{{0}}}", String.Join(", ", Value.Select((entry) => entry.Key + ": " + entry.Value).ToArray()));
        }

        protected bool Equals(CborMap other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CborMap) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(CborMap left, CborMap right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CborMap left, CborMap right)
        {
            return !Equals(left, right);
        }
    }
}
