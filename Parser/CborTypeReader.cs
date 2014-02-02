// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborTypeReader.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor object read completion handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Parser
{
    using System;

    using DiverseWorlds.Logic.Network.Cbor.Exception;

    /// <summary>
    /// The cbor object read completion handler.
    /// </summary>
    public delegate void CborObjectReadCompletionHandler();

    /// <summary>
    /// The cbor type reader.
    /// </summary>
    public abstract class CborTypeReader : CborReaderListener
    {
        #region Fields

        /// <summary>
        /// The reader.
        /// </summary>
        protected readonly CborReader reader;

        /// <summary>
        /// The next type.
        /// </summary>
        protected Type nextType;

        /// <summary>
        /// The current tag.
        /// </summary>
        private uint currentTag;

        /// <summary>
        /// The inner type reader.
        /// </summary>
        private CborTypeReader innerTypeReader;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborTypeReader"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        protected CborTypeReader(CborReader reader)
        {
            this.reader = reader;
            reader.Listener = this;
            this.nextType = null;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The complete event.
        /// </summary>
        public event CborObjectReadCompletionHandler CompleteEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The on array.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnArray(int size)
        {
            if (this.currentTag != 0)
            {
                throw new CborException("invalid tagging on type");
            }

            this.innerTypeReader = new CborListReader(this.reader, size, this.nextType);

            this.innerTypeReader.CompleteEvent += this.InnerTypeReaderOnCompleteEvent;
            this.reader.Listener = this.innerTypeReader;
        }

        /// <summary>
        /// The on bytes.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnBytes(byte[] value)
        {
            if (this.currentTag != 0)
            {
                throw new CborException("invalid tagging on type");
            }

            this.OnObject(value);
        }

        /// <summary>
        /// The on double.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnDouble(double value)
        {
            if (this.currentTag != 0)
            {
                throw new CborException("invalid tagging on type");
            }

            this.OnObject(value);
        }

        /// <summary>
        /// The on integer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="sign">
        /// The sign.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnInteger(uint value, int sign)
        {
            if (this.currentTag == 1)
            {
                // datetime
                DateTime dateTime = DateTimeExtensions.DateTimeFromUnixTimestampSeconds(sign * value);
                this.OnObject(dateTime);
                this.currentTag = 0;
                return;
            }

            if (this.currentTag != 0)
            {
                throw new CborException("invalid tagging on type");
            }

            this.OnObject(sign * (int)value); // TODO: check overflow
        }

        /// <summary>
        /// The on long.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="sign">
        /// The sign.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnLong(ulong value, int sign)
        {
            // logger.info("onLong");
            if (this.currentTag == 1)
            {
                // datetime
                DateTime dateTime = DateTimeExtensions.DateTimeFromUnixTimestampSeconds(sign * (long)value);
                this.OnObject(dateTime);
                this.currentTag = 0;
                return;
            }

            if (this.currentTag != 0)
            {
                throw new CborException("invalid tagging on type");
            }

            this.OnObject(sign * (long)value); // TODO: check overflow
        }

        /// <summary>
        /// The on map.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnMap(int size)
        {
            if (this.currentTag != 0)
            {
                // parse object
                CborTypeTemplate innerTemplate = CborTypeRegistry.Instance.GetTemplate(this.currentTag);

                if (innerTemplate == null)
                {
                    throw new CborException("unknown object tag: " + this.currentTag);
                }

                this.currentTag = 0;

                this.innerTypeReader = new CborObjectReader(this.reader, innerTemplate, size);
                this.innerTypeReader.CompleteEvent += this.InnerTypeReaderOnCompleteEvent;
                this.reader.Listener = this.innerTypeReader;
            }
            else
            {
                // parse map
                this.innerTypeReader = new CborMapReader(this.reader, size, this.nextType);
                this.innerTypeReader.CompleteEvent += this.InnerTypeReaderOnCompleteEvent;
                this.reader.Listener = this.innerTypeReader;
            }
        }

        /// <summary>
        /// The on object.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        public abstract void OnObject(object obj);

        /// <summary>
        /// The on special.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnSpecial(uint code)
        {
            if (this.currentTag != 0)
            {
                throw new CborException("invalid tagging on type");
            }

            switch (code)
            {
                case 20: // false
                    this.OnObject(false);
                    break;
                case 21: // true
                    this.OnObject(true);
                    break;
                case 22: // null
                    this.OnObject(null);
                    break;
                default:
                    throw new CborException("unknown special value");
            }
        }

        /// <summary>
        /// The on string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnString(string value)
        {
            if (this.currentTag != 0)
            {
                throw new CborException("invalid tagging on type");
            }

            this.OnObject(value);
        }

        /// <summary>
        /// The on tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void OnTag(uint tag)
        {
            if (this.currentTag != 0)
            {
                throw new CborException("invalid tagging on type");
            }

            this.currentTag = tag;
        }

        /// <summary>
        /// The result.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public abstract object Result();

        #endregion

        #region Methods

        /// <summary>
        /// The on compete.
        /// </summary>
        protected void OnCompete()
        {
            this.CompleteEvent();
        }

        /// <summary>
        /// The inner type reader on complete event.
        /// </summary>
        private void InnerTypeReaderOnCompleteEvent()
        {
            this.reader.Listener = this;
            this.OnObject(this.innerTypeReader.Result());
            this.innerTypeReader = null;
        }

        #endregion
    }
}