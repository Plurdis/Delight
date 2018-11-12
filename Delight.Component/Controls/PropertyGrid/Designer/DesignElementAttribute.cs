using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Controls
{
    public class DesignElementAttribute : Attribute
    {
        public string Key { get; set; }

        public bool Visible { get; set; } = true;

        public string DisplayName { get; set; }

        public int DisplayIndex { get; set; }

        // 속성 타입이 Enum일경우만 적용됨
        public bool IsNotEnum { get; set; } = true;

        public string Category { get; set; }

        public string[] ToolBoxCategories { get; set; } = new[] { "Default" };

        public bool Generate { get; set; } = true;
    }
}
