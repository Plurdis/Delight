using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StagePainter.Core.Common;

namespace StagePainter.Core.Attributes
{
    /// <summary>
    /// Set Storage Method and Encoding Types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class StorageMethodAttribute : Attribute
    {
        public StorageMethodTypes StorageMethodTypes { get; }

        public DataEncodingTypes DataEncodingTypes { get; }

        /// <summary>
        /// Initalize <see cref="StorageMethodAttribute"/> Class with StorageMethodTypes and DataEncodingTypes.
        /// </summary>
        /// <param name="methodTypes"></param>
        /// <param name="encodingTypes"></param>
        public StorageMethodAttribute(StorageMethodTypes methodTypes, DataEncodingTypes encodingTypes)
        {
            StorageMethodTypes = methodTypes;
            DataEncodingTypes = encodingTypes;
        }

        /// <summary>
        /// Initalize <see cref="StorageMethodAttribute"/> Class with StorageMethodTypes (No Encoding).
        /// </summary>
        /// <param name="methodTypes"></param>
        /// <param name="encodingTypes"></param>
        public StorageMethodAttribute(StorageMethodTypes methodTypes) : this(methodTypes, DataEncodingTypes.None)
        {
        }
    }
}
