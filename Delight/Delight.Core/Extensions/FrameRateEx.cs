using Delight.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Extensions
{
    public static class FrameRateEx
    {
        /// <summary>
        /// FrameRate가 가지고 있는 Int 값를 가져옵니다.
        /// </summary>
        /// <param name="frameRate"></param>
        /// <returns></returns>
        public static int ToInt32(this FrameRate frameRate)
        {
            return (int)frameRate.GetAttribute<DefaultValueAttribute>().Value;
        }
    }
}
