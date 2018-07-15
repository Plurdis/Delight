using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Exceptions
{
    /// <summary>
    /// 어떤 작업중에 예외가 발생하면 그 예외를 내부 예외로 가지고 나타나는 예외입니다.
    /// </summary>
    [Serializable]
    public class ProcessException : Exception
    {
        public ProcessException() { }
        public ProcessException(string message) : base(message) { }
        public ProcessException(string message, Exception inner) : base(message, inner) { }
        protected ProcessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
