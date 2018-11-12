using Delight.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Controls
{
    internal class PropertyGridItemModel
    {
        internal AttributeTuple<DesignElementAttribute, PropertyInfo> Metadata { get; }

        public string Title => Metadata.Attribute.DisplayName;

        public string Category => Metadata.Attribute.Category;

        public ISetter Setter { get; }

        public PropertyGridItemModel(AttributeTuple<DesignElementAttribute, PropertyInfo> data, ISetter setter)
        {
            this.Metadata = data;
            this.Setter = setter;
        }
    }
}
