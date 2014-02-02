// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborObjectReader.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor object reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Parser
{
    using System;

    /// <summary>
    /// The cbor object reader.
    /// </summary>
    internal class CborObjectReader : CborTypeReader
    {
        #region Fields

        /// <summary>
        /// The obj.
        /// </summary>
        private readonly object obj;

        /// <summary>
        /// The template.
        /// </summary>
        private readonly CborTypeTemplate template;

        /// <summary>
        /// The current key.
        /// </summary>
        private string currentKey;

        /// <summary>
        /// The current size.
        /// </summary>
        private int currentSize;

        /// <summary>
        /// The inner type reader.
        /// </summary>
        private CborTypeReader innerTypeReader;

        /// <summary>
        /// The state.
        /// </summary>
        private ParseState state;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborObjectReader"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="innerTemplate">
        /// The inner template.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        public CborObjectReader(CborReader reader, CborTypeTemplate innerTemplate, int size)
            : base(reader)
        {
            this.template = innerTemplate;
            this.currentSize = size;
            this.obj = Activator.CreateInstance(this.template.type);
            this.state = ParseState.KEY;
            this.currentKey = null;
        }

        #endregion

        #region Enums

        /// <summary>
        /// The parse state.
        /// </summary>
        private enum ParseState
        {
            /// <summary>
            /// The key.
            /// </summary>
            KEY, 

            /// <summary>
            /// The value.
            /// </summary>
            VALUE
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The on object.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void OnObject(object value)
        {
            // Debug.Log("object on object: state " + state + " value " + obj);
            if (this.state == ParseState.KEY)
            {
                this.currentKey = (string)value; // TODO: checks
                this.nextType = this.template.GetPropertyType(this.currentKey);
                this.state = ParseState.VALUE;
            }
            else
            {
                // Debug.Log("set key " + currentKey + " value " + value);
                this.template.SetValue(this.obj, this.currentKey, value);
                this.currentSize--;
                this.nextType = null;
                this.state = ParseState.KEY;

                if (this.currentSize == 0)
                {
                    this.OnCompete();
                }
            }
        }

        /// <summary>
        /// The result.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Result()
        {
            return this.obj;
        }

        #endregion
    }
}