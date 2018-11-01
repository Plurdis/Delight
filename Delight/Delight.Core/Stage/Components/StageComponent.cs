using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Stage.Components
{
    public abstract class StageComponent
    {
        public StageComponent(SourceType sourceType, bool isDynamicLength = false)
        {
            SourceType = sourceType;
            IsDynamicLength = isDynamicLength;
        }

        /// <summary>
        /// 해당 컴포넌트의 소스 타입을 나타냅니다.
        /// </summary>
        public SourceType SourceType { get; } = SourceType.Unknown;

        /// <summary>
        /// 해당 컴포넌트의 보여지는 시간을 나타냅니다.
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// StageComponent의 식별자입니다.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 썸네일의 Uri 경로를 나타냅니다.
        /// </summary>
        public Uri Thumbnail { get; set; }

        /// <summary>
        /// 해당 길이가 동적인 길이인지에 대해서 나타냅니다.
        /// </summary>
        public bool IsDynamicLength { get; } = false;
    }
}
