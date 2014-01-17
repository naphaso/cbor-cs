using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.DiverseWorlds.Cbor.Parser {
    class CborMapReader : CborTypeReader {
        private int currentSize;
        private IDictionary dictionary;
        private ParseState state;
        private object firstKey;
        private object firstValue;
        private object currentKey;
        private enum ParseState {
            FIRST_KEY, FIRST_VALUE,
            KEY, VALUE
        }
        public CborMapReader(CborReader reader, int size, Type targeType) : base(reader) {
            this.currentSize = size;
            state = ParseState.FIRST_KEY;
            dictionary = (IDictionary) Activator.CreateInstance(targeType);
        }

        public override object Result() {
            return dictionary;
        }

        public override void OnObject(object obj) {
            switch(state) {
                case ParseState.FIRST_KEY:
                    firstKey = obj;
                    state = ParseState.FIRST_VALUE;
                    break;
                case ParseState.FIRST_VALUE:
                    firstValue = obj;
                    //dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(firstKey.GetType(), firstValue.GetType()));
                    dictionary.Add(firstKey, firstValue);
                    currentSize--;
                    state = ParseState.KEY;
                    break;
                case ParseState.KEY:
                    currentKey = obj;
                    state = ParseState.VALUE;
                    break;
                case ParseState.VALUE:
                    dictionary.Add(currentKey, obj);
                    currentSize--;
                    state = ParseState.KEY;
                    break;
            }

            if(currentSize == 0) {
                OnCompete();
            }
        }
    }
}
