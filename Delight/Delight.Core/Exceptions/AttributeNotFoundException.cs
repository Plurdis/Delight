using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Exceptions
{
    /// <summary>
    /// <see cref="Attribute"/>를 찾지 못했을때 발생하는 예외입니다.
    /// </summary>
    [Serializable]
    public class AttributeNotFoundException : Exception
    {
        public AttributeNotFoundException() { }
        public AttributeNotFoundException(string message) : base(message) { }
        public AttributeNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected AttributeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
