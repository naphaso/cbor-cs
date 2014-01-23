using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.DiverseWorlds.Cbor.Parser {
    class CborListReader : CborTypeReader {
        private int currentSize;
        private int elementIndex;
        private IList list;
        private Array array;
        public CborListReader(CborReader reader, int size, Type targetType) : base(reader) {
            currentSize = size;
            elementIndex = 0;
            if (targetType.IsArray) {
                array = Array.CreateInstance(targetType.GetElementType(), size);
            } else {
                list = (IList)Activator.CreateInstance(targetType);    
            }
        }

        public override object Result() {
            return list ?? array;
        }

        public override void OnObject(object obj) {
            //if(list == null) {
            //    list = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(obj.GetType()));
            //}

            if (list != null) {
                list.Add(obj);
            } else {
                array.SetValue(obj, elementIndex);
            }
            

            elementIndex++;
            if(currentSize == elementIndex) {
                OnCompete();
            }
        }
    }
}
