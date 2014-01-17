using System.Text;
using Assets.DiverseWorlds.Cbor.Buffer;
using Assets.DiverseWorlds.Cbor.Exception;

namespace Assets.DiverseWorlds.Cbor {
    internal enum CborReaderState {
        Type,
        Pint,
        Nint,
        BytesSize,
        BytesData,
        StringSize,
        StringData,
        Array,
        Map,
        Tag,
        Special
    }

    public class CborReader {
        private readonly CborInput _input;
        private CborReaderListener _listener;
        private int _size; // current element size in bytes
        private CborReaderState _state;

        public CborReader(CborInput input) {
            _input = input;
            //_listener = listener;

            _input.InputEvent += InputOnInputEvent;
            //InputOnInputEvent();
        }

        public CborReaderListener Listener {
            get { return _listener; }
            set { _listener = value; }
        }

        private void InputOnInputEvent() {
            while(TryStep()) {
                // parsing step
            }
        }

        private bool TryStep() {
            switch(_state) {
                case CborReaderState.Type:
                    return TryParseType();
                case CborReaderState.Pint:
                    return TryParsePint();
                case CborReaderState.Nint:
                    return TryParseNint();
                case CborReaderState.BytesSize:
                    return TryParseBytesSize();
                case CborReaderState.BytesData:
                    return TryParseBytesData();
                case CborReaderState.StringSize:
                    return TryParseStringSize();
                case CborReaderState.StringData:
                    return TryParseStringData();
                case CborReaderState.Array:
                    return TryParseArray();
                case CborReaderState.Map:
                    return TryParseMap();
                case CborReaderState.Tag:
                    return TryParseTag();
                case CborReaderState.Special:
                    return TryParseSpecial();
                default:
                    return false;
            }
        }

        private bool TryParseType() {
            if(!_input.HasBytes(1)) {
                return false;
            }

            int type = _input.GetByte();
            int majorType = type >> 5;
            int minorType = type & 31;

            switch(majorType) {
                case 0: // positive integer
                    if(minorType < 24) {
                        _listener.OnInteger((uint) minorType, 1);
                    } else if(minorType == 24) {
                        _state = CborReaderState.Pint;
                        _size = 1;
                    } else if(minorType == 25) {
                        _state = CborReaderState.Pint;
                        _size = 2;
                    } else if(minorType == 26) {
                        _state = CborReaderState.Pint;
                        _size = 4;
                    } else if(minorType == 27) {
                        _state = CborReaderState.Pint;
                        _size = 8;
                    } else {
                        throw new CborException("invalid positive integer constructor");
                    }
                    break;
                case 1: // negative integer
                    if(minorType < 24) {
                        _listener.OnInteger((uint) minorType, -1);
                    } else if(minorType == 24) {
                        _state = CborReaderState.Nint;
                        _size = 1;
                    } else if(minorType == 25) {
                        _state = CborReaderState.Nint;
                        _size = 2;
                    } else if(minorType == 26) {
                        _state = CborReaderState.Nint;
                        _size = 4;
                    } else if(minorType == 27) {
                        _state = CborReaderState.Nint;
                        _size = 8;
                    } else {
                        throw new CborException("invalid negative integer constructor");
                    }
                    break;
                case 2: // bytes
                    if(minorType < 24) {
                        _state = CborReaderState.BytesData;
                        _size = minorType;
                    } else if(minorType == 24) {
                        _state = CborReaderState.BytesSize;
                        _size = 1;
                    } else if(minorType == 25) {
                        _state = CborReaderState.BytesSize;
                        _size = 2;
                    } else if(minorType == 26) {
                        _state = CborReaderState.BytesSize;
                        _size = 4;
                    } else if(minorType == 27) {
                        _state = CborReaderState.BytesSize;
                        _size = 8;
                    } else {
                        throw new CborException("invalid bytes constructor");
                    }
                    break;
                case 3: // string
                    if(minorType < 24) {
                        _state = CborReaderState.StringData;
                        _size = minorType;
                    } else if(minorType == 24) {
                        _state = CborReaderState.StringSize;
                        _size = 1;
                    } else if(minorType == 25) {
                        _state = CborReaderState.StringSize;
                        _size = 2;
                    } else if(minorType == 26) {
                        _state = CborReaderState.StringSize;
                        _size = 4;
                    } else if(minorType == 27) {
                        _state = CborReaderState.StringSize;
                        _size = 8;
                    } else {
                        throw new CborException("invalid string constructor");
                    }
                    break;
                case 4: // array
                    if(minorType < 24) {
                        _listener.OnArray(minorType);
                    } else if(minorType == 24) {
                        _state = CborReaderState.Array;
                        _size = 1;
                    } else if(minorType == 25) {
                        _state = CborReaderState.Array;
                        _size = 2;
                    } else if(minorType == 26) {
                        _state = CborReaderState.Array;
                        _size = 4;
                    } else if(minorType == 27) {
                        _state = CborReaderState.Array;
                        _size = 8;
                    } else {
                        throw new CborException("invalid array constructor");
                    }
                    break;
                case 5: // map
                    if(minorType < 24) {
                        _listener.OnMap(minorType);
                    } else if(minorType == 24) {
                        _state = CborReaderState.Map;
                        _size = 1;
                    } else if(minorType == 25) {
                        _state = CborReaderState.Map;
                        _size = 2;
                    } else if(minorType == 26) {
                        _state = CborReaderState.Map;
                        _size = 4;
                    } else if(minorType == 27) {
                        _state = CborReaderState.Map;
                        _size = 8;
                    } else {
                        throw new CborException("invalid map constructor");
                    }
                    break;
                case 6: // tag
                    if(minorType < 24) {
                        _listener.OnTag((uint) minorType);
                    } else if(minorType == 24) {
                        _state = CborReaderState.Tag;
                        _size = 1;
                    } else if(minorType == 25) {
                        _state = CborReaderState.Tag;
                        _size = 2;
                    } else if(minorType == 26) {
                        _state = CborReaderState.Tag;
                        _size = 4;
                    } else if(minorType == 27) {
                        _state = CborReaderState.Tag;
                        _size = 8;
                    } else {
                        throw new CborException("invalid map constructor");
                    }
                    break;
                case 7: // special
                    if(minorType < 24) {
                        _listener.OnSpecial((uint) minorType);
                    } else if(minorType == 24) {
                        _state = CborReaderState.Special;
                        _size = 1;
                    } else if(minorType == 25) {
                        _state = CborReaderState.Special;
                        _size = 2;
                    } else if(minorType == 26) {
                        _state = CborReaderState.Special;
                        _size = 4;
                    } else if(minorType == 27) {
                        _state = CborReaderState.Special;
                        _size = 8;
                    } else {
                        throw new CborException("invalid map constructor");
                    }
                    break;
            }

            return true;
        }

        private bool TryParseTag() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            switch(_size) {
                case 1:
                    _listener.OnTag(_input.GetInt8());
                    break;
                case 2:
                    _listener.OnTag(_input.GetInt16());
                    break;
                case 4:
                    _listener.OnTag(_input.GetInt32());
                    break;
                case 8:
                    throw new CborException("8 bytes tags not supported");
                default:
                    throw new CborException("invalid tag size");
            }

            _state = CborReaderState.Type;

            return true;
        }

        private bool TryParseSpecial() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            switch(_size) {
                case 1:
                    _listener.OnSpecial(_input.GetInt8());
                    break;
                case 2:
                    _listener.OnSpecial(_input.GetInt16());
                    break;
                case 4:
                    _listener.OnSpecial(_input.GetInt32());
                    break;
                case 8:
                    throw new CborException("8 bytes special codes not supported");
                default:
                    throw new CborException("invalid special code size");
            }

            _state = CborReaderState.Type;

            return true;
        }

        private bool TryParseMap() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            switch(_size) {
                case 1:
                    _listener.OnMap((int) _input.GetInt8());
                    break;
                case 2:
                    _listener.OnMap((int) _input.GetInt16());
                    break;
                case 4:
                    _listener.OnMap((int) _input.GetInt32());
                    break;
                case 8:
                    throw new CborException("8 bytes maps not supported");
                default:
                    throw new CborException("invalid map size");
            }

            _state = CborReaderState.Type;

            return true;
        }

        private bool TryParseArray() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            switch(_size) {
                case 1:
                    _listener.OnArray((int) _input.GetInt8());
                    break;
                case 2:
                    _listener.OnArray((int) _input.GetInt16());
                    break;
                case 4:
                    _listener.OnArray((int) _input.GetInt32());
                    break;
                case 8:
                    throw new CborException("8 bytes arrays not supported");
                default:
                    throw new CborException("invalid array size");
            }

            _state = CborReaderState.Type;

            return true;
        }

        private bool TryParseStringData() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            _listener.OnString(Encoding.UTF8.GetString(_input.GetBytes(_size)));
            _state = CborReaderState.Type;
            return true;
        }

        private bool TryParseStringSize() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            switch(_size) {
                case 1:
                    _size = (int) _input.GetInt8();
                    break;
                case 2:
                    _size = (int) _input.GetInt16();
                    break;
                case 4:
                    _size = (int) _input.GetInt32();
                    break;
                case 8:
                    throw new CborException("8 bytes string size not supported");
                default:
                    throw new CborException("invalid string size");
            }

            _state = CborReaderState.StringData;

            return true;
        }

        private bool TryParseBytesData() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            _listener.OnBytes(_input.GetBytes(_size));
            _state = CborReaderState.Type;
            return true;
        }

        private bool TryParseBytesSize() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            switch(_size) {
                case 1:
                    _size = (int) _input.GetInt8();
                    break;
                case 2:
                    _size = (int) _input.GetInt16();
                    break;
                case 4:
                    _size = (int) _input.GetInt32();
                    break;
                case 8:
                    throw new CborException("8 bytes bytes size not supported");
                default:
                    throw new CborException("invalid bytes size");
            }

            _state = CborReaderState.BytesData;

            return true;
        }

        private bool TryParseNint() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            switch(_size) {
                case 1:
                    _listener.OnInteger(_input.GetInt8(), -1);
                    break;
                case 2:
                    _listener.OnInteger(_input.GetInt16(), -1);
                    break;
                case 4:
                    _listener.OnInteger(_input.GetInt32(), -1);
                    break;
                case 8:
                    _listener.OnLong(_input.GetInt64(), -1);
                    break;
                default:
                    throw new CborException("invalid negative integer size");
            }

            _state = CborReaderState.Type;

            return true;
        }

        private bool TryParsePint() {
            if(!_input.HasBytes(_size)) {
                return false;
            }

            switch(_size) {
                case 1:
                    _listener.OnInteger(_input.GetInt8(), 1);
                    break;
                case 2:
                    _listener.OnInteger(_input.GetInt16(), 1);
                    break;
                case 4:
                    _listener.OnInteger(_input.GetInt32(), 1);
                    break;
                case 8:
                    _listener.OnLong(_input.GetInt64(), 1);
                    break;
                default:
                    throw new CborException("invalid positive integer size");
            }

            _state = CborReaderState.Type;

            return true;
        }
    }
}