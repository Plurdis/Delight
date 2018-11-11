using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.PropertyEditor
{
    public class AxisEditor : ExtendedPropertyValueEditor
    {
        public AxisEditor()
        {
            this.InlineEditorTemplate = Application.Current.FindResource("AxisTemplate") as DataTemplate;
        }
    }
}
