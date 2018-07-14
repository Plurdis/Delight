using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.IO
{
    /// <summary>
    /// 인코딩 유형과 저장할 방법을 설정하는 특성입니다. 상속될 수 없습니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class StorageMethodAttribute : Attribute
    {
        public StorageMethodTypes StorageMethodType { get; }

        public Encoding EncodingType { get; }

        /// <summary>
        /// <see cref="StorageMethodAttribute"/> 클래스를 저장 방법과 인코딩 타입으로 초기화합니다.
        /// </summary>
        /// <param name="storageMethodType"></param>
        /// <param name="encodingType"></param>
        public StorageMethodAttribute(StorageMethodTypes storageMethodType, Encoding encodingType)
        {
            StorageMethodType = storageMethodType;
            EncodingType = encodingType;
        }

        /// <summary>
        /// <see cref="StorageMethodAttribute"/> 클래스를 저장 방법으로 초기화합니다.
        /// </summary>
        /// <param name="methodType"></param>
        public StorageMethodAttribute(StorageMethodTypes methodType) : this(methodType, Encoding.Unicode)
        {
        }
    }
}
