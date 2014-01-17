using System;

namespace Assets.DiverseWorlds.Cbor.Parser {
    class CborObjectReader : CborTypeReader {
        private readonly CborTypeTemplate template;
        private object obj;
        private ParseState state;
        private string currentKey;
        private int currentSize;
        private CborTypeReader innerTypeReader;
        private enum ParseState {
            KEY, VALUE
        }
        public CborObjectReader(CborReader reader, CborTypeTemplate innerTemplate, int size) : base(reader) {
            template = innerTemplate;
            currentSize = size;
            this.obj = Activator.CreateInstance(template.type);
            state = ParseState.KEY;
            currentKey = null;
        }

        public override object Result() {
            return obj;
        }

        public override void OnObject(object value) {
            //Debug.Log("object on object: state " + state + " value " + obj);
            if(state == ParseState.KEY) {
                currentKey = (string) value; // TODO: checks
                nextType = template.GetPropertyType(currentKey);
                state = ParseState.VALUE;
            } else {
                //Debug.Log("set key " + currentKey + " value " + value);
                template.SetValue(obj, currentKey, value);
                currentSize--;
                nextType = null;
                state = ParseState.KEY;

                if(currentSize == 0) {
                    OnCompete();
                }
            }
        }
    }
}
