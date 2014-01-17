using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Assets.DiverseWorlds.Cbor;
using Assets.DiverseWorlds.Cbor.Buffer;
using Assets.DiverseWorlds.Cbor.Exception;
using Assets.DiverseWorlds.Cbor.Parser;
using DiverseWorlds.Cbor;

namespace DiverseWorlds.Cbor {
    /*
    public interface CborSerializer {
        void Serialize(object obj, CborWriter writer);
    }

    public abstract class CborAbstractSerializer<T> : CborSerializer {
        protected abstract void Serialize(T obj, CborWriter writer);
        public void Serialize(object obj, CborWriter writer) {
            Serialize((T)obj, writer);
        }
    }

    public class CborStringSerializer : CborAbstractSerializer<string> {

        protected override void Serialize(string obj, CborWriter writer) {
            writer.Write(obj);
        }
    }

    public class CborMapSerializer: CborAbstractSerializer<> {
        protected void Serialize<K, V>(Dictionary<K, V> obj, CborWriter writer) {
            
        }
    }*/


    

    public class Cbor {
        public static byte[] Serialize(object obj) {
            using(MemoryStream memory = new MemoryStream()) {
                WriteObjectToStream(obj, memory);
                return memory.ToArray();
            }
        }
        public static void WriteObjectToStream(object obj, Stream output) {
            CborTypeTemplate template = CborTypeRegistry.Instance.GetTemplate(obj.GetType());
            if(template == null) {
                throw new CborException("unknown object type to serialization");
            }

            using(CborWriter writer = new CborWriter(output)) {
                WriteObjectWithTemplate(obj, template, writer);
            }
        }

        public static void WriteTemplateObject(object obj, CborWriter writer) {
            CborTypeTemplate template = CborTypeRegistry.Instance.GetTemplate(obj.GetType());
            if (template == null) {
                throw new CborException("unknown object type to serialization: " + obj.GetType());
            }

            WriteObjectWithTemplate(obj, template, writer);
        }

        public static void WriteObject(object obj, CborWriter writer) {
            string objString = obj as string;
            if(objString != null) {
                writer.Write(objString);
                return;
            }

            if(obj is int) {
                writer.Write((int)obj);
                return;
            }

            if(obj is long) {
                writer.Write((long)obj);
                return;
            }

            byte[] objBytes = obj as byte[];
            if(objBytes != null) {
                writer.Write(objBytes);
                return;
            }

            IDictionary objDict = obj as IDictionary;
            if(objDict != null) {
                writer.WriteMap(objDict.Count);
                foreach (DictionaryEntry entry in objDict) {
                    WriteObject(entry.Key, writer);
                    WriteObject(entry.Value, writer);
                }
                return;
            }

            IList objList = obj as IList;
            if(objList != null) {
                writer.WriteArray(objList.Count);
                foreach (var element in objList) {
                    WriteObject(obj, writer);
                }
                return;
            }

            CborTypeTemplate template = CborTypeRegistry.Instance.GetTemplate(obj.GetType());
            if (template == null) {
                throw new CborException("unknown object type to serialization: " + obj.GetType());
            }

            WriteObjectWithTemplate(obj, template, writer);
            
            /*

            if (type == typeof(string)) {
                writer.Write((string)obj);
            } else if(type == typeof(int)) {
                writer.Write((int) obj);
            } else if(type == typeof(long)) {
                writer.Write((long) obj); 
            } else if(type == typeof(byte[])) {
                writer.Write((byte[]) obj);
            } else if(obj is IDictionary) {
                IDictionary dict = (IDictionary) obj;
                writer.Write(dict.Count);
                foreach(DictionaryEntry entry in dict) {
                    WriteObjectWithType(entry.Key, entry.Key.GetType(), writer);
                    WriteObjectWithType(entry.Value, entry.Value.GetType(), writer);
                }
            } else if(obj is IList) {
                IList list = (IList) obj;
                writer.Write(list.Count);
                foreach(var element in list) {
                    WriteObjectWithType(obj, obj.GetType(), writer);
                }
            */
                /* } else if(type.IsGenericType) {
                Type genericType = type.GetGenericTypeDefinition();
                if(typeof(Dictionary<,>).IsAssignableFrom(genericType)) {
                    //Debug.Log("writing map: " + obj + ", generic arguments: " + genericType.GetGenericArguments()[0].get() + ", " + genericType.GetGenericArguments()[1].GetElementType());
                    Debug.Log("generic method: " + typeof(Cbor).GetMethod("WriteMap").MakeGenericMethod(genericType.GetGenericArguments()));
                    typeof(Cbor).GetMethod("WriteMap").MakeGenericMethod(genericType.GetGenericArguments()).Invoke(null, new[] {obj, writer});
                } else if(typeof(List<>).IsAssignableFrom(genericType)) {
                    Debug.Log("writing list: " + obj);
                    typeof(Cbor).GetMethod("WriteList").MakeGenericMethod(genericType.GetGenericArguments()).Invoke(null, new[] {obj, writer});
                }
             *//*
            } else {
                CborTypeTemplate template = CborTypeRegistry.Instance.GetTemplate(type);
                if (template == null) {
                    throw new CborException("unknown object type to serialization: " + type.Name);
                }

                WriteObjectWithTemplate(obj, template, writer);
            }*/
        }


        private static void WriteObjectWithTemplate(object obj, CborTypeTemplate template, CborWriter writer) {
            writer.WriteTag(template.tag);
            writer.WriteMap(template.properties.Length);

            foreach (var propertyTemplate in template.properties) {
                writer.Write(propertyTemplate.name);
                WriteObject(propertyTemplate.property.GetValue(obj, null), writer);
            }
        }

        public static void WriteMap<K, V>(Dictionary<K, V> dictionary, CborWriter writer) {
            writer.WriteMap(dictionary.Count);

            foreach(var v in dictionary) {
                WriteObject(v.Key, writer);
                WriteObject(v.Value, writer);
            }
        }

        public static void WriteList<T>(List<T> list, CborWriter writer) {
            writer.WriteArray(list.Count);
            foreach(var element in list) {
                WriteObject(element, writer);
            }
        }

        
        

    }

    // deserialization


    /*
    public class CborListReader : CborTypeReader {
        private readonly CborReader reader;
        private int currentSize;
        private IList list;

        public CborListReader(CborReader reader, int size) {
            this.reader = reader;
            this.currentSize = size;
        }

        public override object Result() {
            return list;
        }

        public override void OnInteger(uint value, int sign) {
            list.Add(sign * (int)value); // TODO: check overflow
        }

        public override void OnLong(ulong value, int sign) {
            list.Add(sign * (long)value); // TODO: check overflow
        }

        public override void OnBytes(byte[] value) {
            list.Add(value);
        }

        public override void OnString(string value) {
            list.Add(value);
        }

        public override void OnArray(int size) {
            throw new NotImplementedException();
        }

        public override void OnMap(int size) {
            throw new NotImplementedException();
        }

        public override void OnTag(uint tag) {
            throw new NotImplementedException();
        }

        public override void OnSpecial(uint code) {
            throw new NotImplementedException();
        }
    }

    public class CborMapReader : CborTypeReader {
        private readonly CborReader reader;
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
        public CborMapReader(CborReader reader, int size) {
            this.reader = reader;
            this.currentSize = size;
            state = ParseState.FIRST_KEY;
        }

        public override object Result() {
            return dictionary;
        }

        public override void OnInteger(uint value, int sign) {
            if(state == ParseState.KEY) {
                currentKey = sign*(int) value; // TODO: check overflow
                state = ParseState.VALUE;
            } else if(state == ParseState.VALUE) {
                dictionary.Add(currentKey, sign * (int)value); // TODO: check overflow
                EndCheck();
            } if(state == ParseState.FIRST_KEY) {
                firstKey = sign*(int) value; // TODO: check overflow
                state = ParseState.FIRST_VALUE;
            } else if(state == ParseState.FIRST_VALUE) {
                firstValue = sign*(int) value;

                dictionary = (IDictionary) Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(firstKey.GetType(), firstValue.GetType()));
                dictionary.Add(firstKey, firstValue);

                EndCheck();
            }
        }

        public override void OnLong(ulong value, int sign) {
            if (state == ParseState.KEY) {
                currentKey = sign * (int)value; // TODO: check overflow
                state = ParseState.VALUE;
            } else if (state == ParseState.VALUE) {
                dictionary.Add(currentKey, sign * (long)value);
                EndCheck();
            } if (state == ParseState.FIRST_KEY) {
                firstKey = sign * (long)value; // TODO: check overflow
                state = ParseState.FIRST_VALUE;
            } else if (state == ParseState.FIRST_VALUE) {
                firstValue = sign * (int)value;

                dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(firstKey.GetType(), firstValue.GetType()));
                dictionary.Add(firstKey, firstValue);

                EndCheck();
            }
        }

        public override void OnBytes(byte[] value) {
            if (state == ParseState.KEY) {
                currentKey = value;
                state = ParseState.VALUE;
            } else if (state == ParseState.VALUE) {
                dictionary.Add(currentKey, value);
                EndCheck();
            } if (state == ParseState.FIRST_KEY) {
                firstKey = value;
                state = ParseState.FIRST_VALUE;
            } else if (state == ParseState.FIRST_VALUE) {
                firstValue = value;

                dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(firstKey.GetType(), firstValue.GetType()));
                dictionary.Add(firstKey, firstValue);

                EndCheck();
            }
        }

        public override void OnString(string value) {
            if (state == ParseState.KEY) {
                currentKey = value;
                state = ParseState.VALUE;
            } else if (state == ParseState.VALUE) {
                dictionary.Add(currentKey, value);
                EndCheck();
            } if (state == ParseState.FIRST_KEY) {
                firstKey = value;
                state = ParseState.FIRST_VALUE;
            } else if (state == ParseState.FIRST_VALUE) {
                firstValue = value;

                dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(firstKey.GetType(), firstValue.GetType()));
                dictionary.Add(firstKey, firstValue);

                EndCheck();
            }
        }

        public override void OnArray(int size) {
            throw new NotImplementedException();
        }

        public override void OnMap(int size) {
            throw new NotImplementedException();
        }

        public override void OnTag(uint tag) {
            throw new NotImplementedException();
        }

        public override void OnSpecial(uint code) {
            throw new NotImplementedException();
        }

        private void EndCheck() {
            currentSize--;
            if (currentSize == 0) {
                OnCompete();
            } else {
                state = ParseState.KEY;
            }
        }
    }

    public class CborObjectTypeReader : CborTypeReader {
        private readonly CborReader reader;
        private readonly CborTypeTemplate template;
        private object obj;
        private ParseState state;
        private string currentKey;
        private uint currentTag;
        private int currentLength;
        private CborTypeReader innerTypeReader;
        private enum ParseState {
            KEY, VALUE
        }
        public CborObjectTypeReader(CborReader reader, CborTypeTemplate template, int length) {
            this.reader = reader;
            this.template = template;
            this.obj = Activator.CreateInstance(template.type);
            state = ParseState.KEY;
            currentKey = null;
            currentTag = 0;
            currentLength = length;
        }

        public override object Result() {
            return obj;
        }

        public override void OnInteger(uint value, int sign) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if (state == ParseState.VALUE) {
                template.SetValue(obj, currentKey, sign*(int)value); // TODO: check overflow
                EndCheck();
            } else throw new CborException("object key is not a string");
        }

        public override void OnLong(ulong value, int sign) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if (state == ParseState.VALUE) {
                template.SetValue(obj, currentKey, sign*(long)value); // TODO: check overflow
                EndCheck();
            } else throw new CborException("object key is not a string");
        }

        public override void OnBytes(byte[] value) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if(state == ParseState.VALUE) {
                template.SetValue(obj, currentKey, value);
                EndCheck();
            } else throw new CborException("object key is not a string");
        }

        public override void OnString(string value) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            if(state == ParseState.KEY) {
                currentKey = value;
                state = ParseState.VALUE;
            } else {
                template.SetValue(obj, currentKey, value);
                EndCheck();
            }
        }

        public override void OnArray(int size) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");
            innerTypeReader = new CborListReader(reader, size);
            innerTypeReader.CompleteEvent += InnerTypeReaderOnCompleteEvent;
            reader.Listener = innerTypeReader;
        }

        public override void OnMap(int size) {
            if(currentTag != 0) { // parse object
                CborTypeTemplate innerTemplate = CborTypeRegistry.Instance.GetTemplate(currentTag);
                
                if(innerTemplate == null) {
                    throw new CborException("unknown object tag: " + currentTag);
                }

                currentTag = 0;

                innerTypeReader = new CborObjectTypeReader(reader, innerTemplate, size);
                innerTypeReader.CompleteEvent += InnerTypeReaderOnCompleteEvent;
                reader.Listener = innerTypeReader;
            } else { // parse map
                innerTypeReader = new CborMapReader(reader, size);
                innerTypeReader.CompleteEvent += InnerTypeReaderOnCompleteEvent;
                reader.Listener = innerTypeReader;
            }
        }

        private void InnerTypeReaderOnCompleteEvent() {
            if (state == ParseState.VALUE) {
                template.SetValue(obj, currentKey, innerTypeReader.Result());
                reader.Listener = this;
                innerTypeReader = null;
                EndCheck();
            } else throw new CborException("object key is not a string");
        }

        public override void OnTag(uint tag) {
            if (currentTag != 0) throw new CborException("invalid tagging on type");

            currentTag = tag;
        }

        public override void OnSpecial(uint code) {
            if(currentTag != 0) throw new CborException("invalid tagging on type");
            if (state == ParseState.VALUE) {
                switch (code) {
                    case 20: // false
                        template.SetValue(obj, currentKey, false);
                        break;
                    case 21: // true
                        template.SetValue(obj, currentKey, true);
                        break;
                    case 22: // null
                        template.SetValue(obj, currentKey, null);
                        break;
                    default:
                        throw new CborException("unknown special value");
                }

                EndCheck();
            } else throw new CborException("object key is not a string");
        }

        private void EndCheck() {
            currentLength--;
            if(currentLength == 0) {
                OnCompete();
            } else {
                state = ParseState.KEY;
            }
        }
    }*/


}
