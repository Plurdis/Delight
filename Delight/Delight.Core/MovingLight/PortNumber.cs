using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight
{
    /// <summary>
    /// 포트 번호를 나타냅니다.
    /// </summary>
    public enum PortNumber
    {
        /// <summary>
        /// X축의 상태를 나타냅니다. (0도~540도)
        /// </summary>
        XAxis = 1,
        /// <summary>
        /// Y축의 상태를 나타냅니다. (0도~270도)
        /// </summary>
        YAxis = 2,
        /// <summary>
        /// 조명의 밝기 정도를 나타냅니다.
        /// </summary>
        Brightness = 3,
        /// <summary>
        /// 조명의 깜빡임 정도를 나타냅니다.
        /// </summary>
        Blink = 4,
        /// <summary>
        /// 조명 효과의 색상을 지정합니다.
        /// </summary>
        Color = 5,
        /// <summary>
        /// 조명 효과의 모양을 결정합니다.
        /// </summary>
        Shape1 = 6,
        ShapeAuto= 7,
        ShakeShape1= 8,
        /// <summary>
        /// Shape1에 덧대여질 모양을 결정합니다.
        /// </summary>
        Shape2 = 9,
        ShakeShape2 = 10,
        Focus = 11,
        /// <summary>
        /// 투영 갯수를 늘립니다. (1개, 3개) 이후에는 3개 상태로 돌아갑니다.
        /// </summary>
        PrismRotate = 12,
        /// <summary>
        /// 자체 움직임의 속도를 조절합니다. 이 값이 높아질수록 처리 속도가 길어집니다.
        /// </summary>
        Speed = 13,
        PanFine = 14,
        /// <summary>
        /// 
        /// </summary>
        TiltFine = 15,
        /// <summary>
        /// 150-200 사이의 값일 경우 조명을 리셋시킵니다.
        /// </summary>
        Reset = 16,
    }
}
