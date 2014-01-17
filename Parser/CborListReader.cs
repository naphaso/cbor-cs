using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.DiverseWorlds.Cbor.Parser {
    class CborListReader : CborTypeReader {
        private int currentSize;
        private IList list; 
        public CborListReader(CborReader reader, int size, Type targetType) : base(reader) {
            currentSize = size;
            list = (IList) Activator.CreateInstance(targetType);
        }

        public override object Result() {
            return list;
        }

        public override void OnObject(object obj) {
            if(list == null) {
                list = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(obj.GetType()));
            }

            list.Add(obj);

            currentSize--;
            if(currentSize == 0) {
                OnCompete();
            }
        }
    }
}
