using Delight.Core.Template.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Delight.Core.Sources
{
    public class ExternalSources
    {
        [XmlElement(typeof(YoutubeSource))]
        public List<BaseSource> Sources { get; set; }

        public ExternalSources()
        {
            Sources = new List<BaseSource>();
        }

        public ExternalSources(IEnumerable<BaseSource> sources)
        {
            Sources = new List<BaseSource>(sources);
        }
    }
}
