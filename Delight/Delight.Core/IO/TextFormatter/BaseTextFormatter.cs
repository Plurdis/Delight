using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.IO.TextFormatter
{
    /// <summary>
    /// 모든 포매터의 기초가 되는 텍스트 포매터입니다.
    /// </summary>
    public abstract class BaseTextFormatter
    {
        public BaseTextFormatter(object obj)
        {
            this.Object = obj;
        }

        public object Object { get; set; }

        public abstract string TextFormat();
    }
}
