// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborField.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Attribute
{
    using System;

    /// <summary>
    /// The cbor field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CborField : Attribute
    {
        #region Fields

        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborField"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public CborField(string name)
        {
            this.name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion
    }
}