using Delight.Core.Template.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Template.Items
{
    public abstract class BaseSource
    {
        public string ThumbnailUri { get; set; }

        public string Title { get; set; }

        public abstract void Download();

        public abstract List<BaseOption> Options { get; set; }
    }
}
