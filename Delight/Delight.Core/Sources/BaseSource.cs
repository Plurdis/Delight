using Delight.Core.Template.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public abstract void Download(int SelectedIndex);

        public abstract List<BaseOption> Options { get; }
    }
}
