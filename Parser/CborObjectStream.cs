// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborObjectStream.cs" company="">
//   
// </copyright>
// <summary>
//   The read object handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Parser
{
    using System;

    using Naphaso.Cbor.Buffer;

    /// <summary>
    /// The read object handler.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    public delegate void ReadObjectHandler(object obj);

    /// <summary>
    /// The cbor object stream.
    /// </summary>
    public class CborObjectStream : CborTypeReader
    {
        #region Fields

        /// <summary>
        /// The input.
        /// </summary>
        private readonly CborInput input;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborObjectStream"/> class.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        public CborObjectStream(CborInput input)
            : base(new CborReader(input))
        {
            this.input = input;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The object event.
        /// </summary>
        public event ReadObjectHandler ObjectEvent;

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
            this.input.AddChunk(chunk);
        }

        /// <summary>
        /// The on object.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        public override void OnObject(object obj)
        {
            this.OnObjectEvent(obj);
        }

        /// <summary>
        /// The result.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override object Result()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on object event.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        protected virtual void OnObjectEvent(object obj)
        {
            ReadObjectHandler handler = this.ObjectEvent;
            if (handler != null)
            {
                handler(obj);
            }
        }

        #endregion
    }
}