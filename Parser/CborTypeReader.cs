using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.DiverseWorlds.Cbor.Exception;

namespace Assets.DiverseWorlds.Cbor.Parser {
    public delegate void CborObjectReadCompletionHandler();
    public abstract class CborTypeReader : CborReaderListener {
        protected readonly CborReader reader;
        public event CborObjectReadCompletionHandler CompleteEvent;
        private CborTypeReader innerTypeReader;
        private uint currentTag;
        protected Type nextType;
        protected CborTypeReader(CborReader reader) {
            this.reader = reader;
            reader.Listener = this;
            nextType = null;
        }

        protected void OnCompete() {
            CompleteEvent();
        }
        public abstract object Result();
        public abstract void OnObject(object obj);

        public void OnInteger(uint value, int sign) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            OnObject(sign * (int)value); // TODO: check overflow
        }

        public void OnLong(ulong value, int sign) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            OnObject(sign * (long)value); // TODO: check overflow
        }

        public void OnBytes(byte[] value) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            OnObject(value);
        }

        public void OnString(string value) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            OnObject(value);
        }

        public void OnArray(int size) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            innerTypeReader = new CborListReader(reader, size, nextType);
            innerTypeReader.CompleteEvent += InnerTypeReaderOnCompleteEvent;
            reader.Listener = innerTypeReader;
        }

        public void OnMap(int size) {
            if (currentTag != 0) { // parse object
                CborTypeTemplate innerTemplate = CborTypeRegistry.Instance.GetTemplate(currentTag);

                if (innerTemplate == null) {
                    throw new CborException("unknown object tag: " + currentTag);
                }

                currentTag = 0;

                innerTypeReader = new CborObjectReader(reader, innerTemplate, size);
                innerTypeReader.CompleteEvent += InnerTypeReaderOnCompleteEvent;
                reader.Listener = innerTypeReader;
            } else { // parse map
                innerTypeReader = new CborMapReader(reader, size, nextType);
                innerTypeReader.CompleteEvent += InnerTypeReaderOnCompleteEvent;
                reader.Listener = innerTypeReader;
            }
        }

        private void InnerTypeReaderOnCompleteEvent() {
            reader.Listener = this;
            OnObject(innerTypeReader.Result());
            innerTypeReader = null;
        }

        public void OnTag(uint tag) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");

            currentTag = tag;
        }

        public void OnSpecial(uint code) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            switch (code) {
                case 20: // false
                    OnObject(false);
                    break;
                case 21: // true
                    OnObject(true);
                    break;
                case 22: // null
                    OnObject(null);
                    break;
                default:
                    throw new CborException("unknown special value");
            }
        }
    }
}
