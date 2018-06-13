using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.IO
{
    /// <summary>
    /// Set Storage Method and Encoding Types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class StorageMethodAttribute : Attribute
    {
        public StorageMethodTypes StorageMethodType { get; }

        public Encoding EncodingType { get; }

        /// <summary>
        /// Initalize <see cref="StorageMethodAttribute"/> Class with StorageMethodTypes and DataEncodingTypes.
        /// </summary>
        /// <param name="storageMethodType"></param>
        /// <param name="encodingType"></param>
        public StorageMethodAttribute(StorageMethodTypes storageMethodType, Encoding encodingType)
        {
            StorageMethodType = storageMethodType;
            EncodingType = encodingType;
        }

        /// <summary>
        /// Initalize <see cref="StorageMethodAttribute"/> Class with StorageMethodTypes (No Encoding).
        /// </summary>
        /// <param name="methodType"></param>
        public StorageMethodAttribute(StorageMethodTypes methodType) : this(methodType, Encoding.Unicode)
        {
        }
    }
}
