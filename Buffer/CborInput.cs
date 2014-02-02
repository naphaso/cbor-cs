// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborInput.cs" company="">
//   
// </copyright>
// <summary>
//   The input handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Buffer
{
    /// <summary>
    /// The input handler.
    /// </summary>
    public delegate void InputHandler();

    /// <summary>
    /// The borInput interface.
    /// </summary>
    public interface CborInput
    {
        #region Public Events

        /// <summary>
        /// The input event.
        /// </summary>
        event InputHandler InputEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add chunk.
        /// </summary>
        /// <param name="chunk">
        /// The chunk.
        /// </param>
        void AddChunk(byte[] chunk);

        /// <summary>
        /// The get byte.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int GetByte();

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] GetBytes(int count);

        /// <summary>
        /// The get int 16.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        uint GetInt16();

        /// <summary>
        /// The get int 32.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        uint GetInt32();

        /// <summary>
        /// The get int 64.
        /// </summary>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        ulong GetInt64();

        /// <summary>
        /// The get int 8.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        uint GetInt8();

        /// <summary>
        /// The has bytes.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool HasBytes(int count);

        #endregion
    }
}