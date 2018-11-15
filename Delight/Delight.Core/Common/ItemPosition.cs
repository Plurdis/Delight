using Delight.Component.ItemProperties;
using Delight.Core.Stage;
using System.Xml.Serialization;

namespace Delight.Component.Common
{
    public class ItemPosition
    {
        public int FrameWidth { get; set; }

        public int Offset { get; set; }

        public int ForwardOffset { get; set; }

        public int BackwardOffset { get; set; }

        public string ItemId { get; set; }

        public SourceType SourceType { get; set; }

        public int TrackNumber { get; set; }
        
        [XmlElement(typeof(LightItemProperty))]
        [XmlElement(typeof(VideoItemProperty))]
        [XmlElement(typeof(BaseLightSetterProperty))]
        public object Property { get; set; }
    }
}
