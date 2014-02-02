// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborMapReader.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor map reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Parser
{
    using System;
    using System.Collections;

    /// <summary>
    /// The cbor map reader.
    /// </summary>
    internal class CborMapReader : CborTypeReader
    {
        #region Fields

        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly IDictionary dictionary;

        /// <summary>
        /// The current key.
        /// </summary>
        private object currentKey;

        /// <summary>
        /// The current size.
        /// </summary>
        private int currentSize;

        /// <summary>
        /// The first key.
        /// </summary>
        private object firstKey;

        /// <summary>
        /// The first value.
        /// </summary>
        private object firstValue;

        /// <summary>
        /// The state.
        /// </summary>
        private ParseState state;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborMapReader"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="targeType">
        /// The targe type.
        /// </param>
        public CborMapReader(CborReader reader, int size, Type targeType)
            : base(reader)
        {
            this.currentSize = size;
            this.state = ParseState.FIRST_KEY;
            this.dictionary = (IDictionary)Activator.CreateInstance(targeType);
        }

        #endregion

        #region Enums

        /// <summary>
        /// The parse state.
        /// </summary>
        private enum ParseState
        {
            /// <summary>
            /// The firs t_ key.
            /// </summary>
            FIRST_KEY, 

            /// <summary>
            /// The firs t_ value.
            /// </summary>
            FIRST_VALUE, 

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
        /// <param name="obj">
        /// The obj.
        /// </param>
        public override void OnObject(object obj)
        {
            switch (this.state)
            {
                case ParseState.FIRST_KEY:
                    this.firstKey = obj;
                    this.state = ParseState.FIRST_VALUE;
                    break;
                case ParseState.FIRST_VALUE:
                    this.firstValue = obj;

                    // dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(firstKey.GetType(), firstValue.GetType()));
                    this.dictionary.Add(this.firstKey, this.firstValue);
                    this.currentSize--;
                    this.state = ParseState.KEY;
                    break;
                case ParseState.KEY:
                    this.currentKey = obj;
                    this.state = ParseState.VALUE;
                    break;
                case ParseState.VALUE:
                    this.dictionary.Add(this.currentKey, obj);
                    this.currentSize--;
                    this.state = ParseState.KEY;
                    break;
            }

            if (this.currentSize == 0)
            {
                this.OnCompete();
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
            return this.dictionary;
        }

        #endregion
    }
}