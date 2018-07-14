using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Tracks
{
    /// <summary>
    /// 트랙 아이템의 가장 기초적인 정보를 담고 있는 인터페이스입니다.
    /// </summary>
    public interface ITrackItemInfo : ICloneable
    {
        double Offset { get; set; }

        double Size { get; }

        string Name { get; set; }
    }
}
