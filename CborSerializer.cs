// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborSerializer.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor serializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using DiverseWorlds.Logic.Network.Cbor.Exception;

    /// <summary>
    /// The cbor serializer.
    /// </summary>
    public class CborSerializer
    {
        #region Public Methods and Operators

        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] Serialize(object obj)
        {
            using (var memory = new MemoryStream())
            {
                WriteObjectToStream(obj, memory);
                return memory.ToArray();
            }
        }

        /// <summary>
        /// The write list.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void WriteList<T>(List<T> list, CborWriter writer)
        {
            writer.WriteArray(list.Count);
            foreach (T element in list)
            {
                WriteObject(element, writer);
            }
        }

        /// <summary>
        /// The write map.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <typeparam name="K">
        /// </typeparam>
        /// <typeparam name="V">
        /// </typeparam>
        public static void WriteMap<K, V>(Dictionary<K, V> dictionary, CborWriter writer)
        {
            writer.WriteMap(dictionary.Count);

            foreach (var v in dictionary)
            {
                WriteObject(v.Key, writer);
                WriteObject(v.Value, writer);
            }
        }

        /// <summary>
        /// The write object.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public static void WriteObject(object obj, CborWriter writer)
        {
            // logger.debug("write some object: {0}", obj);
            if (obj == null)
            {
                writer.writeSpecial(22);
                return;
            }

            var objBytes = obj as byte[];
            if (objBytes != null)
            {
                writer.Write(objBytes);
                return;
            }

            var objArray = obj as Array;
            if (objArray != null)
            {
                writer.WriteArray(objArray.Length);
                foreach (object element in objArray)
                {
                    WriteObject(element, writer);
                }

                return;
            }

            var objString = obj as string;
            if (objString != null)
            {
                writer.Write(objString);
                return;
            }

            if (obj is bool)
            {
                writer.writeSpecial((bool)obj ? 21u : 20u);
                return;
            }

            if (obj is uint)
            {
                writer.Write((uint)obj);
                return;
            }

            if (obj is int || obj.GetType().IsEnum)
            {
                writer.Write((int)obj);
                return;
            }

            if (obj is ulong)
            {
                writer.Write((ulong)obj);
                return;
            }

            if (obj is long)
            {
                writer.Write((long)obj);
                return;
            }

            if (obj is double)
            {
                writer.Write((double)obj);
                return;
            }

            if (obj is DateTime)
            {
                writer.WriteTag(1);
                writer.Write(((DateTime)obj).GetUnixTimestampSeconds());
                return;
            }

            var objDict = obj as IDictionary;
            if (objDict != null)
            {
                writer.WriteMap(objDict.Count);
                foreach (DictionaryEntry entry in objDict)
                {
                    WriteObject(entry.Key, writer);
                    WriteObject(entry.Value, writer);
                }

                return;
            }

            var objList = obj as IList;
            if (objList != null)
            {
                writer.WriteArray(objList.Count);
                foreach (object element in objList)
                {
                    WriteObject(obj, writer);
                }

                return;
            }

            CborTypeTemplate template = CborTypeRegistry.Instance.GetTemplate(obj.GetType());
            if (template == null)
            {
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
                    Debug.Log("generic method: " + typeof(CborSerializer).GetMethod("WriteMap").MakeGenericMethod(genericType.GetGenericArguments()));
                    typeof(CborSerializer).GetMethod("WriteMap").MakeGenericMethod(genericType.GetGenericArguments()).Invoke(null, new[] {obj, writer});
                } else if(typeof(List<>).IsAssignableFrom(genericType)) {
                    Debug.Log("writing list: " + obj);
                    typeof(CborSerializer).GetMethod("WriteList").MakeGenericMethod(genericType.GetGenericArguments()).Invoke(null, new[] {obj, writer});
                }
             */ /*
            } else {
                CborTypeTemplate template = CborTypeRegistry.Instance.GetTemplate(type);
                if (template == null) {
                    throw new CborException("unknown object type to serialization: " + type.Name);
                }

                WriteObjectWithTemplate(obj, template, writer);
            }*/
        }

        /// <summary>
        /// The write object to stream.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public static void WriteObjectToStream(object obj, Stream output)
        {
            // logger.debug("write object to stream");
            CborTypeTemplate template = CborTypeRegistry.Instance.GetTemplate(obj.GetType());
            if (template == null)
            {
                throw new CborException("unknown object type to serialization");
            }

            using (var writer = new CborWriter(output))
            {
                WriteObjectWithTemplate(obj, template, writer);
            }
        }

        /// <summary>
        /// The write template object.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public static void WriteTemplateObject(object obj, CborWriter writer)
        {
            CborTypeTemplate template = CborTypeRegistry.Instance.GetTemplate(obj.GetType());
            if (template == null)
            {
                throw new CborException("unknown object type to serialization: " + obj.GetType());
            }

            WriteObjectWithTemplate(obj, template, writer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The write object with template.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private static void WriteObjectWithTemplate(object obj, CborTypeTemplate template, CborWriter writer)
        {
            // logger.debug("write object with template: {0}", obj);
            writer.WriteTag(template.tag);
            writer.WriteMap(template.properties.Length);

            // logger.info("writing object with {0} fields", template.properties.Length);
            foreach (CborPropertyTemplate propertyTemplate in template.properties)
            {
                // logger.info("writing property name {0} value {1}", propertyTemplate.name, propertyTemplate.property.GetValue(obj,null));
                writer.Write(propertyTemplate.name);
                WriteObject(propertyTemplate.property.GetValue(obj, null), writer);
            }
        }

        #endregion
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