// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborException.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DiverseWorlds.Logic.Network.Cbor.Exception
{
    using System;

    /// <summary>
    /// The cbor exception.
    /// </summary>
    internal class CborException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public CborException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CborException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public CborException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}