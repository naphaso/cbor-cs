// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborReader.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor reader state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor
{
    using System;
    using System.Text;

    using DiverseWorlds.Logic.Network.Cbor.Exception;

    using Naphaso.Cbor.Buffer;

    /// <summary>
    /// The cbor reader state.
    /// </summary>
    internal enum CborReaderState
    {
        /// <summary>
        /// The type.
        /// </summary>
        Type, 

        /// <summary>
        /// The pint.
        /// </summary>
        Pint, 

        /// <summary>
        /// The nint.
        /// </summary>
        Nint, 

        /// <summary>
        /// The bytes size.
        /// </summary>
        BytesSize, 

        /// <summary>
        /// The bytes data.
        /// </summary>
        BytesData, 

        /// <summary>
        /// The string size.
        /// </summary>
        StringSize, 

        /// <summary>
        /// The string data.
        /// </summary>
        StringData, 

        /// <summary>
        /// The array.
        /// </summary>
        Array, 

        /// <summary>
        /// The map.
        /// </summary>
        Map, 

        /// <summary>
        /// The tag.
        /// </summary>
        Tag, 

        /// <summary>
        /// The special.
        /// </summary>
        Special, 

        /// <summary>
        /// The float data.
        /// </summary>
        FloatData, 
    }

    /// <summary>
    /// The cbor reader.
    /// </summary>
    public class CborReader
    {
        #region Fields

        /// <summary>
        /// The _input.
        /// </summary>
        private readonly CborInput _input;

        /// <summary>
        /// The _listener.
        /// </summary>
        private CborReaderListener _listener;

        /// <summary>
        /// The _size.
        /// </summary>
        private int _size; // current element size in bytes

        /// <summary>
        /// The _state.
        /// </summary>
        private CborReaderState _state;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborReader"/> class.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        public CborReader(CborInput input)
        {
            this._input = input;

            // _listener = listener;
            this._input.InputEvent += this.InputOnInputEvent;

            // InputOnInputEvent();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the listener.
        /// </summary>
        public CborReaderListener Listener
        {
            get
            {
                return this._listener;
            }

            set
            {
                this._listener = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The input on input event.
        /// </summary>
        private void InputOnInputEvent()
        {
            while (this.TryStep())
            {
                // parsing step
            }
        }

        /// <summary>
        /// The try parse array.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseArray()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            switch (this._size)
            {
                case 1:
                    this._listener.OnArray((int)this._input.GetInt8());
                    break;
                case 2:
                    this._listener.OnArray((int)this._input.GetInt16());
                    break;
                case 4:
                    this._listener.OnArray((int)this._input.GetInt32());
                    break;
                case 8:
                    throw new CborException("8 bytes arrays not supported");
                default:
                    throw new CborException("invalid array size");
            }

            this._state = CborReaderState.Type;

            return true;
        }

        /// <summary>
        /// The try parse bytes data.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TryParseBytesData()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            this._listener.OnBytes(this._input.GetBytes(this._size));
            this._state = CborReaderState.Type;
            return true;
        }

        /// <summary>
        /// The try parse bytes size.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseBytesSize()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            switch (this._size)
            {
                case 1:
                    this._size = (int)this._input.GetInt8();
                    break;
                case 2:
                    this._size = (int)this._input.GetInt16();
                    break;
                case 4:
                    this._size = (int)this._input.GetInt32();
                    break;
                case 8:
                    throw new CborException("8 bytes bytes size not supported");
                default:
                    throw new CborException("invalid bytes size");
            }

            this._state = CborReaderState.BytesData;

            return true;
        }

        /// <summary>
        /// The try parse float.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseFloat()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            byte[] data = this._input.GetBytes(8);

            // logger.info("parse float: {0}", BitConverter.ToString(data).Replace("-","").ToLower());
            switch (this._size)
            {
                case 8: // double
                    this._listener.OnDouble(BitConverter.ToDouble(data, 0));
                    break;
                default:
                    throw new CborException("invalid float size");
            }

            this._state = CborReaderState.Type;

            return true;
        }

        /// <summary>
        /// The try parse map.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseMap()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            switch (this._size)
            {
                case 1:
                    this._listener.OnMap((int)this._input.GetInt8());
                    break;
                case 2:
                    this._listener.OnMap((int)this._input.GetInt16());
                    break;
                case 4:
                    this._listener.OnMap((int)this._input.GetInt32());
                    break;
                case 8:
                    throw new CborException("8 bytes maps not supported");
                default:
                    throw new CborException("invalid map size");
            }

            this._state = CborReaderState.Type;

            return true;
        }

        /// <summary>
        /// The try parse nint.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseNint()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            switch (this._size)
            {
                case 1:
                    this._listener.OnInteger(this._input.GetInt8(), -1);
                    break;
                case 2:
                    this._listener.OnInteger(this._input.GetInt16(), -1);
                    break;
                case 4:
                    this._listener.OnInteger(this._input.GetInt32(), -1);
                    break;
                case 8:
                    this._listener.OnLong(this._input.GetInt64(), -1);
                    break;
                default:
                    throw new CborException("invalid negative integer size");
            }

            this._state = CborReaderState.Type;

            return true;
        }

        /// <summary>
        /// The try parse pint.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParsePint()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            switch (this._size)
            {
                case 1:
                    this._listener.OnInteger(this._input.GetInt8(), 1);
                    break;
                case 2:
                    this._listener.OnInteger(this._input.GetInt16(), 1);
                    break;
                case 4:
                    this._listener.OnInteger(this._input.GetInt32(), 1);
                    break;
                case 8:
                    this._listener.OnLong(this._input.GetInt64(), 1);
                    break;
                default:
                    throw new CborException("invalid positive integer size");
            }

            this._state = CborReaderState.Type;

            return true;
        }

        /// <summary>
        /// The try parse special.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseSpecial()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            uint code;

            switch (this._size)
            {
                case 1:
                    code = this._input.GetInt8();
                    break;
                case 2:
                    code = this._input.GetInt16();
                    break;
                case 4:
                    code = this._input.GetInt32();
                    break;
                case 8:
                    throw new CborException("8 bytes special codes not supported");
                default:
                    throw new CborException("invalid special code size");
            }

            switch (code)
            {
                case 27: // double
                    this._size = 8;
                    this._state = CborReaderState.FloatData;
                    break;
                default:
                    this._listener.OnSpecial(code);
                    this._state = CborReaderState.Type;
                    break;
            }

            return true;
        }

        /// <summary>
        /// The try parse string data.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TryParseStringData()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            this._listener.OnString(Encoding.UTF8.GetString(this._input.GetBytes(this._size)));
            this._state = CborReaderState.Type;
            return true;
        }

        /// <summary>
        /// The try parse string size.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseStringSize()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            switch (this._size)
            {
                case 1:
                    this._size = (int)this._input.GetInt8();
                    break;
                case 2:
                    this._size = (int)this._input.GetInt16();
                    break;
                case 4:
                    this._size = (int)this._input.GetInt32();
                    break;
                case 8:
                    throw new CborException("8 bytes string size not supported");
                default:
                    throw new CborException("invalid string size");
            }

            this._state = CborReaderState.StringData;

            return true;
        }

        /// <summary>
        /// The try parse tag.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseTag()
        {
            if (!this._input.HasBytes(this._size))
            {
                return false;
            }

            switch (this._size)
            {
                case 1:
                    this._listener.OnTag(this._input.GetInt8());
                    break;
                case 2:
                    this._listener.OnTag(this._input.GetInt16());
                    break;
                case 4:
                    this._listener.OnTag(this._input.GetInt32());
                    break;
                case 8:
                    throw new CborException("8 bytes tags not supported");
                default:
                    throw new CborException("invalid tag size");
            }

            this._state = CborReaderState.Type;

            return true;
        }

        /// <summary>
        /// The try parse type.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        private bool TryParseType()
        {
            if (!this._input.HasBytes(1))
            {
                return false;
            }

            int type = this._input.GetByte();
            int majorType = type >> 5;
            int minorType = type & 31;

            switch (majorType)
            {
                case 0: // positive integer
                    if (minorType < 24)
                    {
                        this._listener.OnInteger((uint)minorType, 1);
                    }
                    else if (minorType == 24)
                    {
                        this._state = CborReaderState.Pint;
                        this._size = 1;
                    }
                    else if (minorType == 25)
                    {
                        this._state = CborReaderState.Pint;
                        this._size = 2;
                    }
                    else if (minorType == 26)
                    {
                        this._state = CborReaderState.Pint;
                        this._size = 4;
                    }
                    else if (minorType == 27)
                    {
                        this._state = CborReaderState.Pint;
                        this._size = 8;
                    }
                    else
                    {
                        throw new CborException("invalid positive integer constructor");
                    }

                    break;
                case 1: // negative integer
                    if (minorType < 24)
                    {
                        this._listener.OnInteger((uint)minorType, -1);
                    }
                    else if (minorType == 24)
                    {
                        this._state = CborReaderState.Nint;
                        this._size = 1;
                    }
                    else if (minorType == 25)
                    {
                        this._state = CborReaderState.Nint;
                        this._size = 2;
                    }
                    else if (minorType == 26)
                    {
                        this._state = CborReaderState.Nint;
                        this._size = 4;
                    }
                    else if (minorType == 27)
                    {
                        this._state = CborReaderState.Nint;
                        this._size = 8;
                    }
                    else
                    {
                        throw new CborException("invalid negative integer constructor");
                    }

                    break;
                case 2: // bytes
                    if (minorType < 24)
                    {
                        this._state = CborReaderState.BytesData;
                        this._size = minorType;
                    }
                    else if (minorType == 24)
                    {
                        this._state = CborReaderState.BytesSize;
                        this._size = 1;
                    }
                    else if (minorType == 25)
                    {
                        this._state = CborReaderState.BytesSize;
                        this._size = 2;
                    }
                    else if (minorType == 26)
                    {
                        this._state = CborReaderState.BytesSize;
                        this._size = 4;
                    }
                    else if (minorType == 27)
                    {
                        this._state = CborReaderState.BytesSize;
                        this._size = 8;
                    }
                    else
                    {
                        throw new CborException("invalid bytes constructor");
                    }

                    break;
                case 3: // string
                    if (minorType < 24)
                    {
                        this._state = CborReaderState.StringData;
                        this._size = minorType;
                    }
                    else if (minorType == 24)
                    {
                        this._state = CborReaderState.StringSize;
                        this._size = 1;
                    }
                    else if (minorType == 25)
                    {
                        this._state = CborReaderState.StringSize;
                        this._size = 2;
                    }
                    else if (minorType == 26)
                    {
                        this._state = CborReaderState.StringSize;
                        this._size = 4;
                    }
                    else if (minorType == 27)
                    {
                        this._state = CborReaderState.StringSize;
                        this._size = 8;
                    }
                    else
                    {
                        throw new CborException("invalid string constructor");
                    }

                    break;
                case 4: // array
                    if (minorType < 24)
                    {
                        this._listener.OnArray(minorType);
                    }
                    else if (minorType == 24)
                    {
                        this._state = CborReaderState.Array;
                        this._size = 1;
                    }
                    else if (minorType == 25)
                    {
                        this._state = CborReaderState.Array;
                        this._size = 2;
                    }
                    else if (minorType == 26)
                    {
                        this._state = CborReaderState.Array;
                        this._size = 4;
                    }
                    else if (minorType == 27)
                    {
                        this._state = CborReaderState.Array;
                        this._size = 8;
                    }
                    else
                    {
                        throw new CborException("invalid array constructor");
                    }

                    break;
                case 5: // map
                    if (minorType < 24)
                    {
                        this._listener.OnMap(minorType);
                    }
                    else if (minorType == 24)
                    {
                        this._state = CborReaderState.Map;
                        this._size = 1;
                    }
                    else if (minorType == 25)
                    {
                        this._state = CborReaderState.Map;
                        this._size = 2;
                    }
                    else if (minorType == 26)
                    {
                        this._state = CborReaderState.Map;
                        this._size = 4;
                    }
                    else if (minorType == 27)
                    {
                        this._state = CborReaderState.Map;
                        this._size = 8;
                    }
                    else
                    {
                        throw new CborException("invalid map constructor");
                    }

                    break;
                case 6: // tag
                    if (minorType < 24)
                    {
                        this._listener.OnTag((uint)minorType);
                    }
                    else if (minorType == 24)
                    {
                        this._state = CborReaderState.Tag;
                        this._size = 1;
                    }
                    else if (minorType == 25)
                    {
                        this._state = CborReaderState.Tag;
                        this._size = 2;
                    }
                    else if (minorType == 26)
                    {
                        this._state = CborReaderState.Tag;
                        this._size = 4;
                    }
                    else if (minorType == 27)
                    {
                        this._state = CborReaderState.Tag;
                        this._size = 8;
                    }
                    else
                    {
                        throw new CborException("invalid map constructor");
                    }

                    break;
                case 7: // special
                    if (minorType < 24)
                    {
                        this._listener.OnSpecial((uint)minorType);
                    }
                    else if (minorType == 24)
                    {
                        this._state = CborReaderState.Special;
                        this._size = 1;
                    }
                    else if (minorType == 25)
                    {
                        this._state = CborReaderState.Special;
                        this._size = 2;
                    }
                    else if (minorType == 26)
                    {
                        this._state = CborReaderState.Special;
                        this._size = 4;
                    }
                    else if (minorType == 27)
                    {
                        this._state = CborReaderState.Special;
                        this._size = 8;
                    }
                    else
                    {
                        throw new CborException("invalid map constructor");
                    }

                    break;
            }

            return true;
        }

        /// <summary>
        /// The try step.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TryStep()
        {
            switch (this._state)
            {
                case CborReaderState.Type:
                    return this.TryParseType();
                case CborReaderState.Pint:
                    return this.TryParsePint();
                case CborReaderState.Nint:
                    return this.TryParseNint();
                case CborReaderState.BytesSize:
                    return this.TryParseBytesSize();
                case CborReaderState.BytesData:
                    return this.TryParseBytesData();
                case CborReaderState.StringSize:
                    return this.TryParseStringSize();
                case CborReaderState.StringData:
                    return this.TryParseStringData();
                case CborReaderState.Array:
                    return this.TryParseArray();
                case CborReaderState.Map:
                    return this.TryParseMap();
                case CborReaderState.Tag:
                    return this.TryParseTag();
                case CborReaderState.Special:
                    return this.TryParseSpecial();

                case CborReaderState.FloatData:
                    return this.TryParseFloat();
                default:
                    return false;
            }
        }

        #endregion
    }
}