using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Delight.Controls
{
    class PresetItem : Control
    {
        public PresetItem()
        {
            this.Style = FindResource("PresetItemStyle") as Style;
        }
    }
}
