using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.DiverseWorlds.Cbor.Buffer;
using Assets.DiverseWorlds.Cbor.Exception;

namespace Assets.DiverseWorlds.Cbor.Parser {
    public delegate void ReadObjectHandler(object obj);
    public class CborObjectStream : CborTypeReader {
        private readonly CborInput input;
        public event ReadObjectHandler ObjectEvent;

        protected virtual void OnObjectEvent(object obj) {
            ReadObjectHandler handler = ObjectEvent;
            if(handler != null) {
                handler(obj);
            }
        }

        public CborObjectStream(CborInput input) : base(new CborReader(input)) {
            this.input = input;
        }

        public void AddChunk(byte[] chunk) {
            input.AddChunk(chunk);
        }


        public override object Result() {
            throw new NotImplementedException();
        }

        public override void OnObject(object obj) {
            OnObjectEvent(obj);
        }
    }
}
