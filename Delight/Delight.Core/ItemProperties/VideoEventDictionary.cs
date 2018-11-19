using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.ItemProperties
{
    public class VideoEventDictionary
    {
        public VideoEventDictionary()
        {
            PropertyDictionary = new Dictionary<string, object>();
        }

        Dictionary<string, object> PropertyDictionary { get; set; }

        public object this[string propertyName]
        {
            get => PropertyDictionary[propertyName];
        }
    }
}
