using StagePainter.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Projects
{
    /// <summary>
    /// 프로젝트 리소스와 프로젝트 데이터에 대해서 나타냅니다.
    /// </summary>
    public class ProjectInfo
    {
        public ProjectInfo()
        {
            MediaManager = new MediaManager();
        }

        public MediaManager MediaManager { get; }

    }
}
