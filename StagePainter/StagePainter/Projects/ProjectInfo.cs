using StagePainter.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Projects
{
    /// <summary>
    /// Represents Project Resources and about project datas.
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
