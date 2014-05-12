// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborObject.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using Naphaso.Cbor.Types.Number;

namespace Naphaso.Cbor.Types
{
    using DiverseWorlds.Logic.Network.Cbor.Exception;

    /// <summary>
    /// The cbor object.
    /// </summary>
    public abstract class CborObject
    {
        #region Public Properties

        /// <summary>
        /// Gets the as array.
        /// </summary>
        /// <exception cref="CborException">
        /// </exception>
        public virtual CborArray AsArray
        {
            get
            {
                throw new CborException("is not a array");
            }
        }

        /// <summary>
        /// Gets the as bytes.
        /// </summary>
        /// <exception cref="CborException">
        /// </exception>
        public virtual CborBytes AsBytes
        {
            get
            {
                throw new CborException("is not a bytes");
            }
        }

        /// <summary>
        /// Gets the as map.
        /// </summary>
        /// <exception cref="CborException">
        /// </exception>
        public virtual CborMap AsMap
        {
            get
            {
                throw new CborException("is not a map");
            }
        }

        /// <summary>
        /// Gets the as number.
        /// </summary>
        /// <exception cref="CborException">
        /// </exception>
        public virtual CborNumber AsNumber
        {
            get
            {
                throw new CborException("is not a number");
            }
        }

        /// <summary>
        /// Gets the as special.
        /// </summary>
        /// <exception cref="CborException">
        /// </exception>
        public virtual CborSpecial AsSpecial
        {
            get
            {
                throw new CborException("is not a special");
            }
        }

        /// <summary>
        /// Gets the as string.
        /// </summary>
        /// <exception cref="CborException">
        /// </exception>
        public virtual CborString AsString
        {
            get
            {
                throw new CborException("is not a string");
            }
        }

        /// <summary>
        /// Gets a value indicating whether is array.
        /// </summary>
        public virtual bool IsArray
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is bytes.
        /// </summary>
        public virtual bool IsBytes
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is map.
        /// </summary>
        public virtual bool IsMap
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is number.
        /// </summary>
        public virtual bool IsNumber
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is special.
        /// </summary>
        public virtual bool IsSpecial
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is string.
        /// </summary>
        public virtual bool IsString
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public CborNumber Tag { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The to string with tag.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string toStringWithTag(string value)
        {
            if (this.Tag == null)
            {
                return value;
            }

            return this.Tag + "#" + value;
        }

        public virtual CborWriter Write(CborWriter writer)
        {
            if (this.Tag != null)
            {
                writer.WriteTag(this.Tag);
            }

            return writer;
        }

        public byte[] Serialize()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                CborWriter writer = new CborWriter(memory);
                this.Write(writer);
                return memory.ToArray();
            }
        }

        #endregion

        public static implicit operator CborObject(int value)
        {
            if (value < 0)
            {
                return new CborNumber32(-1, (uint)-value);
            }
            else
            {
                return new CborNumber32(1, (uint)value);
            }
        }

        public static implicit operator CborObject(bool value)
        {
            return value ? CborSpecial.True : CborSpecial.False;
        }

        public static implicit operator CborObject(long value)
        {
            if (value < 0)
            {
                return new CborNumber64(-1, (ulong)-value);
            }
            else
            {
                return new CborNumber64(1, (ulong)value);
            }
        }

        public static implicit operator CborObject(uint value)
        {
            return new CborNumber32(1, value);
        }

        public static implicit operator CborObject(ulong value)
        {
            return new CborNumber64(1, value);
        }

        public static implicit operator CborObject(double value)
        {
            return new CborNumberDouble(value);
        }

        public static implicit operator CborObject(string value)
        {
            return new CborString(value);
        }
    }
}