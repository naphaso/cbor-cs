// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborWriter.cs" company="Naphaso">
//   
// </copyright>
// <summary>
//   The cbor writer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The cbor writer.
    /// </summary>
    public class CborWriter : IDisposable
    {
        #region Fields

        /// <summary>
        /// The output.
        /// </summary>
        private readonly Stream output;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborWriter"/> class.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        public CborWriter(Stream output)
        {
            this.output = output;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(int value)
        {
            if (value < 0)
            {
                this.WriteTypeAndValue(1, (uint)-value);
            }
            else
            {
                this.WriteTypeAndValue(0, (uint)value);
            }
        }

        public void Write(int sign, uint value)
        {
            if (sign < 0)
            {
                this.WriteTypeAndValue(1, value);
            }
            else
            {
                this.WriteTypeAndValue(0, value);
            }
        }

        public void Write(int sign, ulong value)
        {
            if (sign < 0)
            {
                this.WriteTypeAndValue(1, value);
            }
            else
            {
                this.WriteTypeAndValue(0, value);
            }
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(uint value)
        {
            this.WriteTypeAndValue(0, value);
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(long value)
        {
            if (value < 0L)
            {
                this.WriteTypeAndValue(1, (ulong)-value);
            }
            else
            {
                this.WriteTypeAndValue(0, (ulong)value);
            }
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(ulong value)
        {
            this.WriteTypeAndValue(0, value);
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(byte[] value)
        {
            this.WriteTypeAndValue(2, (uint)value.Length);
            this.output.Write(value, 0, value.Length);
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(double value)
        {
            this.WriteTypeAndValue(7, 27u);
            byte[] data = BitConverter.GetBytes(value);

            // logger.info("write double date: {0}", BitConverter.ToString(data).Replace("-","").ToLower());
            this.output.Write(data, 0, 8);
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        public void Write(byte[] value, int offset, int limit)
        {
            this.WriteTypeAndValue(2, (uint)limit);
            this.output.Write(value, offset, limit);
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            this.WriteTypeAndValue(3, (uint)data.Length);
            this.output.Write(data, 0, data.Length);
        }

        /// <summary>
        /// The write array.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        public void WriteArray(int size)
        {
            this.WriteTypeAndValue(4, (uint)size);
        }

        /// <summary>
        /// The write map.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        public void WriteMap(int size)
        {
            this.WriteTypeAndValue(5, (uint)size);
        }

        /// <summary>
        /// The write tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        public void WriteTag(uint tag)
        {
            WriteTypeAndValue(6, tag);
        }

        /// <summary>
        /// The write special.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        public void writeSpecial(uint code)
        {
            WriteTypeAndValue(7, code);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The write type and value.
        /// </summary>
        /// <param name="majorType">
        /// The major type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void WriteTypeAndValue(int majorType, uint value)
        {
            majorType <<= 5;
            if (value < 24)
            {
                this.output.WriteByte((byte)((uint)majorType | value));
            }
            else if (value < 256)
            {
                this.output.WriteByte((byte)(majorType | 24));
                this.output.WriteByte((byte)value);
            }
            else if (value < 65536)
            {
                this.output.WriteByte((byte)(majorType | 25));
                this.output.WriteByte((byte)(value >> 8));
                this.output.WriteByte((byte)value);
            }
            else
            {
                this.output.WriteByte((byte)(majorType | 26));
                this.output.WriteByte((byte)(value >> 24));
                this.output.WriteByte((byte)(value >> 16));
                this.output.WriteByte((byte)(value >> 8));
                this.output.WriteByte((byte)value);
            }
        }

        /// <summary>
        /// The write type and value.
        /// </summary>
        /// <param name="majorType">
        /// The major type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void WriteTypeAndValue(int majorType, ulong value)
        {
            majorType <<= 5;
            if (value < 24UL)
            {
                this.output.WriteByte((byte)(majorType | (int)value));
            }
            else if (value < 256UL)
            {
                this.output.WriteByte((byte)(majorType | 24));
                this.output.WriteByte((byte)value);
            }
            else if (value < 65536UL)
            {
                this.output.WriteByte((byte)(majorType | 25));
                this.output.WriteByte((byte)(value >> 8));
                this.output.WriteByte((byte)value);
            }
            else if (value < 4294967295UL)
            {
                this.output.WriteByte((byte)(majorType | 26));
                this.output.WriteByte((byte)(value >> 24));
                this.output.WriteByte((byte)(value >> 16));
                this.output.WriteByte((byte)(value >> 8));
                this.output.WriteByte((byte)value);
            }
            else
            {
                this.output.WriteByte((byte)(majorType | 27));
                this.output.WriteByte((byte)(value >> 56));
                this.output.WriteByte((byte)(value >> 48));
                this.output.WriteByte((byte)(value >> 40));
                this.output.WriteByte((byte)(value >> 32));
                this.output.WriteByte((byte)(value >> 24));
                this.output.WriteByte((byte)(value >> 16));
                this.output.WriteByte((byte)(value >> 8));
                this.output.WriteByte((byte)value);
            }
        }

        #endregion
    }
}