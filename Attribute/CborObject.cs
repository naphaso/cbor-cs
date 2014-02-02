// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborObject.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Attribute
{
    using System;

    /// <summary>
    /// The cbor object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CborObject : Attribute
    {
        #region Fields

        /// <summary>
        /// The tag.
        /// </summary>
        private readonly uint tag;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborObject"/> class.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        public CborObject(object tag)
        {
            this.tag = (uint)tag;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the tag.
        /// </summary>
        public uint Tag
        {
            get
            {
                return this.tag;
            }
        }

        #endregion
    }
}