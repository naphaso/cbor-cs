// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborReaderListener.cs" company="">
//   
// </copyright>
// <summary>
//   The borReaderListener interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor
{
    /// <summary>
    /// The borReaderListener interface.
    /// </summary>
    public interface CborReaderListener
    {
        #region Public Methods and Operators

        /// <summary>
        /// The on array.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        void OnArray(int size);

        /// <summary>
        /// The on bytes.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void OnBytes(byte[] value);

        /// <summary>
        /// The on double.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void OnDouble(double value);

        /// <summary>
        /// The on integer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="sign">
        /// The sign.
        /// </param>
        void OnInteger(uint value, int sign);

        /// <summary>
        /// The on long.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="sign">
        /// The sign.
        /// </param>
        void OnLong(ulong value, int sign);

        /// <summary>
        /// The on map.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        void OnMap(int size);

        /// <summary>
        /// The on special.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        void OnSpecial(uint code);

        /// <summary>
        /// The on string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void OnString(string value);

        /// <summary>
        /// The on tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        void OnTag(uint tag);

        #endregion
    }
}