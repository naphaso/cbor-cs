// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborInputMoveBuffer.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor input move buffer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Buffer
{
    using System;

    /// <summary>
    /// The cbor input move buffer.
    /// </summary>
    internal class CborInputMoveBuffer : CborInput
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
        /// Initializes a new instance of the <see cref="CborInputMoveBuffer"/> class.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        public CborInputMoveBuffer(int size)
        {
            this.buffer = new byte[size];
            this.readIndex = 0;
            this.writeIndex = 0;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The input event.
        /// </summary>
        public event InputHandler InputEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add chunk.
        /// </summary>
        /// <param name="chunk">
        /// The chunk.
        /// </param>
        public void AddChunk(byte[] chunk)
        {
            if (this.buffer.Length - this.writeIndex < chunk.Length)
            {
                if (this.writeIndex - this.readIndex != 0)
                {
                    Array.Copy(this.buffer, this.readIndex, this.buffer, 0, this.writeIndex - this.readIndex);
                    this.writeIndex = this.writeIndex - this.readIndex;
                    this.readIndex = 0;
                }
                else
                {
                    this.readIndex = 0;
                    this.writeIndex = 0;
                }
            }

            Array.Copy(chunk, 0, this.buffer, this.writeIndex, chunk.Length);
            this.writeIndex += chunk.Length;
            this.InputEvent();
        }

        /// <summary>
        /// The get byte.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetByte()
        {
            return this.buffer[this.readIndex++];
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
            var data = new byte[count];
            Array.Copy(this.buffer, this.readIndex, data, 0, count);
            this.readIndex += count;
            return data;
        }

        /// <summary>
        /// The get int 16.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetInt16()
        {
            this.readIndex += 2;
            return ((uint)this.buffer[this.readIndex - 2] << 8) | this.buffer[this.readIndex - 1];
        }

        /// <summary>
        /// The get int 32.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetInt32()
        {
            this.readIndex += 4;
            return ((uint)this.buffer[this.readIndex - 4] << 24) | ((uint)this.buffer[this.readIndex - 3] << 16)
                   | ((uint)this.buffer[this.readIndex - 2] << 8) | this.buffer[this.readIndex - 1];
        }

        /// <summary>
        /// The get int 64.
        /// </summary>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        public ulong GetInt64()
        {
            this.readIndex += 8;
            return ((ulong)this.buffer[this.readIndex - 8] << 56) | ((ulong)this.buffer[this.readIndex - 7] << 48)
                   | ((ulong)this.buffer[this.readIndex - 6] << 40) | ((ulong)this.buffer[this.readIndex - 5] << 32)
                   | ((ulong)this.buffer[this.readIndex - 4] << 24) | ((ulong)this.buffer[this.readIndex - 3] << 16)
                   | ((ulong)this.buffer[this.readIndex - 2] << 8) | this.buffer[this.readIndex - 1];
        }

        /// <summary>
        /// The get int 8.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetInt8()
        {
            return this.buffer[this.readIndex++];
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
            return count <= this.writeIndex - this.readIndex;
        }

        #endregion
    }
}