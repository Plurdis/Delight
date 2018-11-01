using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.PropertyEditor
{
    class ColorEditor : ExtendedPropertyValueEditor
    {
        public ColorEditor()
        {
            this.InlineEditorTemplate = Application.Current.FindResource("ColorTemplate") as DataTemplate;
        }
    }
}
