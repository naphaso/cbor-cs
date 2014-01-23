using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.DiverseWorlds.Cbor.Exception;
using Telegram.Core.Logging;
using TestServer.Cbor;

namespace Assets.DiverseWorlds.Cbor.Parser {
    public delegate void CborObjectReadCompletionHandler();
    public abstract class CborTypeReader : CborReaderListener {
        private static readonly Logger logger = LoggerFactory.getLogger(typeof(CborTypeReader));
        private const bool Debug = false;

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
            
            if (currentTag == 1) { // datetime
                DateTime dateTime = DateTimeExtensions.DateTimeFromUnixTimestampSeconds(sign * (long)value);
                if (Debug) { logger.info("datetime: {0}", dateTime); }
                OnObject(dateTime);
                currentTag = 0;
                return;
            }

            if (currentTag != 0) throw new CborException("invalid tagging on type");

            if (Debug) { logger.info("integer: {0}", sign * (int)value); }
            OnObject(sign * (int)value); // TODO: check overflow
        }

        public void OnLong(ulong value, int sign) {
            //logger.info("onLong");
            if (currentTag == 1) { // datetime
                DateTime dateTime = DateTimeExtensions.DateTimeFromUnixTimestampSeconds(sign*(long) value);
                if (Debug) { logger.info("datetime: {0}", dateTime); }
                OnObject(dateTime);
                currentTag = 0;
                return;
            }

            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if (Debug) { logger.info("long: {0}", sign * (long)value); }
            OnObject(sign * (long)value); // TODO: check overflow
        }

        public void OnBytes(byte[] value) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if (Debug) { logger.info("bytes: {0}", BitConverter.ToString(value).Replace("-","").ToLower()); }
            OnObject(value);
        }

        public void OnString(string value) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if (Debug) { logger.info("string: \"{0}\"", value); }
            OnObject(value);
        }

        public void OnArray(int size) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if (Debug) { logger.info("array: {0}", size); }
            innerTypeReader = new CborListReader(reader, size, nextType);
            
            innerTypeReader.CompleteEvent += InnerTypeReaderOnCompleteEvent;
            reader.Listener = innerTypeReader;
        }

        public void OnMap(int size) {
            if (Debug) { logger.info("map: {0}, tag {1}", size, currentTag); }
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

            if (Debug) { logger.info("tag: {0}", tag); }

            currentTag = tag;
        }

        public void OnSpecial(uint code) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if (Debug) { logger.info("special: {0}", code); }
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

        public void OnDouble(double value) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");

            if (Debug) { logger.info("double: {0}", value); }

            OnObject(value);
        }
    }
}
