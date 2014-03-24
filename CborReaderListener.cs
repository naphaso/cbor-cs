// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborReaderListener.cs" company="">
//   
// </copyright>
// <summary>
//   The borReaderListener interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

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

    public class CborReaderDebugListener : CborReaderListener
    {
        public void OnArray(int size)
        {
            Console.WriteLine("on array: {0}", size);
        }

        public void OnBytes(byte[] value)
        {
            Console.WriteLine("on bytes: {0}", BitConverter.ToString(value).Replace("-", "").ToLower());
        }

        public void OnDouble(double value)
        {
            Console.WriteLine("on double: {0}", value);
        }

        public void OnInteger(uint value, int sign)
        {
            Console.WriteLine("on integer: {0}{1}", sign < 0 ? "-" : "", value);
        }

        public void OnLong(ulong value, int sign)
        {
            Console.WriteLine("on long: {0}{1}", sign < 0 ? "-" : "", value);
        }

        public void OnMap(int size)
        {
            Console.WriteLine("on map: {0}", size);
        }

        public void OnSpecial(uint code)
        {
            Console.WriteLine("on special: {0}", code);
        }

        public void OnString(string value)
        {
            Console.WriteLine("on string: {0}", value);
        }

        public void OnTag(uint tag)
        {
            Console.WriteLine("on tag: {0}", tag);
        }
    }
}