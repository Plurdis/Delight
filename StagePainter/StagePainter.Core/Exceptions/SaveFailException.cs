using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.Exceptions
{
    [Serializable]
    public class SaveFailException : Exception
    {
        public SaveFailException() { }
        public SaveFailException(string message) : base(message) { }
        public SaveFailException(string message, Exception inner) : base(message, inner) { }
        protected SaveFailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
