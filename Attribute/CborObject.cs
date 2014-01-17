using System;

namespace Assets.DiverseWorlds.Cbor.Attribute {
    [AttributeUsage(AttributeTargets.Class)]
    public class CborObject : System.Attribute {
        private uint tag;

        public CborObject(uint tag) {
            this.tag = tag;
        }

        public uint Tag {
            get { return tag; }
        }
    }
}
