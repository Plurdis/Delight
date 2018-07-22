using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Delight.Components.Media;

namespace Delight.Projects
{
    /// <summary>
    /// 프로젝트 리소스와 프로젝트 데이터에 대해서 나타냅니다.
    /// </summary>
    public class ProjectInfo : INotifyPropertyChanged
    {
        public ProjectInfo()
        {
            MediaManager = new MediaManager();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 프로젝트 리소스의 초기값을 가져옵니다. (새 프로젝트시 사용)
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo GetDefault()
        {
            return new ProjectInfo();
        }

        private string _projectName;
        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProjectName"));
            }
        }

        public MediaManager MediaManager { get; }

    }
}
