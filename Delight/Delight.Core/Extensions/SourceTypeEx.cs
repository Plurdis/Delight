using Delight.Core.Attributes;
using Delight.Core.Extensions;
using Delight.Core.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Extensions
{
    public static class SourceTypeEx
    {
        public static bool IsVisualSource(this SourceType sourceType)
        {
            return sourceType.GetEnumAttribute<IsVisualTrackAttribute>().IsVisualTrack;
        }
    }
}
