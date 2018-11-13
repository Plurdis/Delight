using Delight.Core.Sources.Options;
using System.Collections.Generic;

namespace Delight.Core.Sources
{
    public abstract class BaseSource
    {
        public BaseSource(string typeText)
        {
            TypeText = typeText;
        }
        public string TypeText { get; }

        public string ThumbnailUri { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 구분할 수 있는 고유한 ID를 나타냅니다.
        /// </summary>
        public string Id { get; set; }

        public abstract bool Download(int SelectedIndex);

        /// <summary>
        /// 템플릿 형태로 데이터를 구조화 시킵니다.
        /// </summary>
        /// <param name="templateName"></param>
        public abstract TemplateData GetTemplateData();

        public abstract List<BaseOption> Options { get; }

        public bool Checked { get; set; }
    }
}
