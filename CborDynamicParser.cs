using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Naphaso.Cbor.Buffer;
using Naphaso.Cbor.Parser;
using Naphaso.Cbor.Types;
using Naphaso.Cbor.Types.Number;

namespace Naphaso.Cbor
{
    public delegate void ObjectHandler(CborObject value);

    internal enum ParseType
    {
        ARRAY, MAP
    }
    internal class DynamicParseState
    {
        public ParseType type;
        public Dictionary<CborObject, CborObject> currentMap;
        public CborObject[] currentArray;
        public CborObject currentKey;
        public CborNumber currentTag;
        public int currentIndex;
        public int maximumIndex;

        public static DynamicParseState ParseArray(int size, CborNumber tag)
        {
            DynamicParseState state = new DynamicParseState();
            state.type = ParseType.ARRAY;
            state.currentArray = new CborObject[size];
            state.currentIndex = 0;
            state.maximumIndex = size;
            state.currentTag = tag;
            return state;
        }

        public static DynamicParseState ParseMap(int size, CborNumber tag)
        {
            DynamicParseState state = new DynamicParseState();
            state.type = ParseType.MAP;
            state.currentMap = new Dictionary<CborObject, CborObject>();
            state.currentIndex = 0;
            state.maximumIndex = size;
            state.currentTag = tag;
            return state;
        }


    }

    public class CborDynamicReader
    {
        private CborInput input;
        private CborReader reader;
        private CborDynamicParser parser;

        public CborInput Input
        {
            get { return input; }
        }

        public CborDynamicParser Parser
        {
            get { return parser; }
        }

        public CborDynamicReader()
        {
            input = new CborInputChunks();
            reader = new CborReader(input);
            parser = new CborDynamicParser();
            reader.Listener = parser;
        }
    }

    public class CborDynamicParser : CborReaderListener
    {
        public event ObjectHandler ObjectEvent;
        private CborNumber currentTag;
        private Stack<DynamicParseState> stack = new Stack<DynamicParseState>();

        public void OnRootObject(CborObject obj)
        {
            var handler = this.ObjectEvent;
            if (handler != null)
            {
                handler(obj);
            }
        }

        public void OnArray(int size)
        {
            if (size == 0)
            {
                CborArray array = new CborArray(0);
                array.Tag = currentTag;
                currentTag = null;
                OnObject(array);
            }
            else
            {
                stack.Push(DynamicParseState.ParseArray(size, currentTag));
                currentTag = null;
            }
        }

        public void OnBytes(byte[] value)
        {
            OnObject(new CborBytes(value));
        }

        public void OnDouble(double value)
        {
            OnObject(new CborNumberDouble(value));
        }

        public void OnInteger(uint value, int sign)
        {
            OnObject(new CborNumber32(sign, value));
        }

        public void OnLong(ulong value, int sign)
        {
            OnObject(new CborNumber64(sign, value));
        }

        public void OnMap(int size)
        {
            if (size == 0)
            {
                CborMap map = new CborMap();
                map.Tag = currentTag;
                currentTag = null;
                OnObject(map);
            }
            else
            {
                stack.Push(DynamicParseState.ParseMap(size, currentTag));
                currentTag = null;
            }
        }

        public void OnSpecial(uint code)
        {
            OnObject(new CborSpecial(code));
        }

        public void OnString(string value)
        {
            OnObject(new CborString(value));
        }

        public void OnObject(CborObject obj)
        {
            if (currentTag != null)
            {
                obj.Tag = currentTag;
                currentTag = null;
            }

            if (stack.Any())
            {
                DynamicParseState state = stack.Peek();
                if (state.type == ParseType.MAP)
                {
                    if (state.currentKey == null)
                    {
                        state.currentKey = obj;
                    }
                    else
                    {
                        state.currentMap.Add(state.currentKey, obj);
                        state.currentKey = null;
                        state.currentIndex++;

                        if (state.currentIndex == state.maximumIndex)
                        {
                            stack.Pop();
                            OnObject(new CborMap(state.currentMap) {Tag = state.currentTag});
                        }
                    }
                }
                else // array
                {
                    state.currentArray[state.currentIndex++] = obj;

                    if (state.currentIndex == state.maximumIndex)
                    {
                        stack.Pop();
                        CborArray array = new CborArray(state.currentArray);
                        array.Tag = state.currentTag;
                        OnObject(array);
                    }
                }
            }
            else
            {
                OnRootObject(obj);
            }
        }

        public void OnTag(uint tag)
        {
            currentTag = new CborNumber32(1, tag);
        }
    }
}
