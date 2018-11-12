using Delight.Core.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Common
{
    public class ItemPosition
    {
        public int Offset { get; set; }

        public int ForwardOffset { get; set; }

        public int BackwardOffset { get; set; }

        public string ItemId { get; set; }

        public SourceType SourceType { get; set; }

        public int TrackNumber { get; set; }
    }
}
