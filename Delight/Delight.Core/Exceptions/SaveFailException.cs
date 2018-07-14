using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Exceptions
{
    /// <summary>
    /// 저장에 실패했을때 발생하는 예외입니다.
    /// </summary>
    [Serializable]
    public class SaveFailException : Exception
    {
        public SaveFailException() { }
        public SaveFailException(string message) : base(message) { }
        public SaveFailException(string message, Exception inner) : base(message, inner) { }
        protected SaveFailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
