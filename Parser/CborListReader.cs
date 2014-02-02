// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborListReader.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor list reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor.Parser
{
    using System;
    using System.Collections;

    /// <summary>
    /// The cbor list reader.
    /// </summary>
    internal class CborListReader : CborTypeReader
    {
        #region Fields

        /// <summary>
        /// The array.
        /// </summary>
        private readonly Array array;

        /// <summary>
        /// The current size.
        /// </summary>
        private readonly int currentSize;

        /// <summary>
        /// The list.
        /// </summary>
        private readonly IList list;

        /// <summary>
        /// The element index.
        /// </summary>
        private int elementIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CborListReader"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        public CborListReader(CborReader reader, int size, Type targetType)
            : base(reader)
        {
            this.currentSize = size;
            this.elementIndex = 0;
            if (targetType.IsArray)
            {
                this.array = Array.CreateInstance(targetType.GetElementType(), size);
            }
            else
            {
                this.list = (IList)Activator.CreateInstance(targetType);
            }
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
            // if(list == null) {
            // list = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(obj.GetType()));
            // }
            if (this.list != null)
            {
                this.list.Add(obj);
            }
            else
            {
                this.array.SetValue(obj, this.elementIndex);
            }

            this.elementIndex++;
            if (this.currentSize == this.elementIndex)
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
            return this.list ?? this.array;
        }

        #endregion
    }
}