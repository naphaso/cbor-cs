// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborInputChunks.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor input chunks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Buffer
{
    using System.Collections.Generic;
    using System.IO;

    using DiverseWorlds.Logic.Network.Cbor.Exception;

    /// <summary>
    /// The cbor input chunks.
    /// </summary>
    public class CborInputChunks : CborInput
    {
        #region Fields

        /// <summary>
        /// The chunks.
        /// </summary>
        private readonly List<MemoryStream> chunks = new List<MemoryStream>();

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
            if (chunk.Length == 0)
            {
                return;
            }

            this.RemoveReadedChunks();
            this.chunks.Add(new MemoryStream(chunk, false));
            this.InputEvent();
        }

        /// <summary>
        /// The get byte.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="CborException">
        /// </exception>
        public int GetByte()
        {
            foreach (MemoryStream memoryStream in this.chunks)
            {
                if (memoryStream.Length - memoryStream.Position > 0)
                {
                    return memoryStream.ReadByte();
                }
            }

            throw new CborException("buffer underflow");
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
        /// <exception cref="CborException">
        /// </exception>
        public byte[] GetBytes(int count)
        {
            var data = new byte[count];
            int collected = 0;
            foreach (MemoryStream chunk in this.chunks)
            {
                var size = (int)(chunk.Length - chunk.Position);
                if (size == 0)
                {
                    continue;
                }

                if (size > count - collected)
                {
                    size = count - collected;
                }

                chunk.Read(data, collected, size);
                collected += size;
                if (collected >= count)
                {
                    return data;
                }
            }

            throw new CborException("buffer underflow");
        }

        /// <summary>
        /// The get int 16.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetInt16()
        {
            // return BitConverter.ToUInt16(GetBytes(2), 0);
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
            // return BitConverter.ToUInt32(GetBytes(4), 0);
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
            return (ulong)((ulong)this.GetByte() << 56) | (ulong)((ulong)this.GetByte() << 48) | (ulong)((ulong)this.GetByte() << 40)
                   | (ulong)((ulong)this.GetByte() << 32) | (ulong)((ulong)this.GetByte() << 24) | (ulong)((ulong)this.GetByte() << 16)
                   | (ulong)((ulong)this.GetByte() << 8) | (ulong)(ulong)this.GetByte();

            // return BitConverter.ToUInt64(GetBytes(8), 0);
        }

        /// <summary>
        /// The get int 8.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetInt8()
        {
            return (uint)this.GetByte();
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
            long collected = 0;
            foreach (MemoryStream memoryStream in this.chunks)
            {
                collected += memoryStream.Length - memoryStream.Position;
                if (collected >= count)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The remove readed chunks.
        /// </summary>
        public void RemoveReadedChunks()
        {
            for (int i = 0; i < this.chunks.Count; i++)
            {
                if (this.chunks[0].Length - this.chunks[0].Position == 0)
                {
                    this.chunks.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
        }

        #endregion
    }
}