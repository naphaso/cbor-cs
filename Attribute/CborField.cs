using System;

namespace Assets.DiverseWorlds.Cbor.Attribute {
    [AttributeUsage(AttributeTargets.Property)]
    public class CborField : System.Attribute {
        private string name;

        public CborField(string name) {
            this.name = name;
        }

        public string Name {
            get { return name; }
        }
    }
}