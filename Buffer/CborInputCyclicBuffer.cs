// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborInputCyclicBuffer.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor input cyclic buffer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Buffer
{
    using System;
    using System.Linq;

    /// <summary>
    /// The cbor input cyclic buffer.
    /// </summary>
    internal class CborInputCyclicBuffer : CborInput
    {
        #region Fields

        /// <summary>
        /// The buffer.
        /// </summary>
        private readonly byte[] buffer;

        /// <summary>
        /// The read index.
        /// </summary>
        private int readIndex;

        /// <summary>
        /// The write index.
        /// </summary>
        private int writeIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborInputCyclicBuffer"/> class.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        public CborInputCyclicBuffer(int size)
        {
            this.buffer = new byte[size];
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The input event.
        /// </summary>
        public event InputHandler InputEvent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the full head.
        /// </summary>
        private int FullHead
        {
            get
            {
                if (this.readIndex <= this.writeIndex)
                {
                    return this.writeIndex - this.readIndex;
                }

                return this.buffer.Length - this.readIndex;
            }
        }

        /// <summary>
        /// Gets the to read.
        /// </summary>
        private int ToRead
        {
            get
            {
                if (this.readIndex <= this.writeIndex)
                {
                    return this.writeIndex - this.readIndex;
                }

                return this.buffer.Length - this.readIndex + this.writeIndex;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add chunk.
        /// </summary>
        /// <param name="chunk">
        /// The chunk.
        /// </param>
        /// <exception cref="OverflowException">
        /// </exception>
        public void AddChunk(byte[] chunk)
        {
            // Debug.Log("add chunk start: r " + readIndex + ", w " + writeIndex);
            if (this.readIndex <= this.writeIndex)
            {
                int headSize = this.buffer.Length - this.writeIndex;
                if (chunk.Length <= headSize)
                {
                    Array.Copy(chunk, 0, this.buffer, this.writeIndex, chunk.Length);
                    this.writeIndex += chunk.Length;
                    if (this.writeIndex == this.buffer.Length)
                    {
                        this.writeIndex = 0;
                    }
                }
                else if (chunk.Length <= headSize + this.readIndex)
                {
                    int tailSize = chunk.Length - headSize;
                    Array.Copy(chunk, 0, this.buffer, this.writeIndex, headSize);
                    Array.Copy(chunk, headSize, this.buffer, 0, tailSize);
                    this.writeIndex = tailSize;
                }
                else
                {
                    throw new OverflowException("cyclic buffer overflow");
                }
            }
            else
            {
                // [----w----r----]
                int headSize = this.readIndex - this.writeIndex;
                if (chunk.Length <= headSize)
                {
                    Array.Copy(chunk, 0, this.buffer, this.writeIndex, chunk.Length);
                    this.writeIndex += chunk.Length;
                }
                else
                {
                    throw new OverflowException("cyclic buffer overflow");
                }
            }

            this.InputEvent();

            // Debug.Log("add chunk end: r " + readIndex + ", w " + writeIndex);
        }

        /// <summary>
        /// The get byte.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetByte()
        {
            byte r = this.buffer[this.readIndex++];
            if (this.readIndex == this.buffer.Count())
            {
                this.readIndex = 0;
            }

            return r;
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] GetBytes(int count)
        {
            var r = new byte[count];
            if (this.FullHead >= count)
            {
                Array.Copy(this.buffer, this.readIndex, r, 0, count);
                this.readIndex += count;
                if (this.readIndex == this.buffer.Length)
                {
                    this.readIndex = 0;
                }
            }
            else
            {
                int fullHead = this.FullHead;
                Array.Copy(this.buffer, this.readIndex, r, 0, fullHead);
                Array.Copy(this.buffer, 0, r, this.FullHead, count - fullHead);
                this.readIndex = count - fullHead;
            }

            return r;
        }

        /// <summary>
        /// The get int 16.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetInt16()
        {
            if (this.FullHead >= 2)
            {
                uint r = ((uint)this.buffer[this.readIndex] << 8) | this.buffer[this.readIndex + 1];
                this.readIndex += 2;
                if (this.readIndex == this.buffer.Length)
                {
                    this.readIndex = 0;
                }

                return r;
            }

            return ((uint)this.GetByte() << 8) | ((uint)this.GetByte());
        }

        /// <summary>
        /// The get int 32.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetInt32()
        {
            if (this.FullHead >= 4)
            {
                uint r = ((uint)this.buffer[this.readIndex] << 24) | ((uint)this.buffer[this.readIndex + 1] << 16)
                         | ((uint)this.buffer[this.readIndex + 2] << 8) | this.buffer[this.readIndex + 3];
                this.readIndex += 4;
                if (this.readIndex == this.buffer.Length)
                {
                    this.readIndex = 0;
                }

                return r;
            }

            return ((uint)this.GetByte() << 24) | ((uint)this.GetByte() << 16) | ((uint)this.GetByte() << 8)
                   | ((uint)this.GetByte());
        }

        /// <summary>
        /// The get int 64.
        /// </summary>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        public ulong GetInt64()
        {
            if (this.FullHead >= 8)
            {
                ulong r = ((ulong)this.buffer[this.readIndex] << 56) | ((ulong)this.buffer[this.readIndex + 1] << 48)
                          | ((ulong)this.buffer[this.readIndex + 2] << 40)
                          | ((ulong)this.buffer[this.readIndex + 3] << 32)
                          | ((ulong)this.buffer[this.readIndex + 4] << 24)
                          | ((ulong)this.buffer[this.readIndex + 5] << 16)
                          | ((ulong)this.buffer[this.readIndex + 6] << 8) | this.buffer[this.readIndex + 7];
                this.readIndex += 8;
                if (this.readIndex == this.buffer.Length)
                {
                    this.readIndex = 0;
                }

                return r;
            }

            return ((ulong)this.GetByte() << 56) | ((ulong)this.GetByte() << 48) | ((ulong)this.GetByte() << 40)
                   | ((ulong)this.GetByte() << 32) | ((ulong)this.GetByte() << 24) | ((ulong)this.GetByte() << 16)
                   | ((ulong)this.GetByte() << 8) | ((ulong)this.GetByte());
        }

        /// <summary>
        /// The get int 8.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetInt8()
        {
            uint r = this.buffer[this.readIndex++];
            if (this.readIndex == this.buffer.Count())
            {
                this.readIndex = 0;
            }

            return r;
        }

        /// <summary>
        /// The has bytes.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasBytes(int count)
        {
            return this.ToRead >= count;
        }

        #endregion
    }
}