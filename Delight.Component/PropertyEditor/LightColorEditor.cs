using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.PropertyEditor
{
    public class LightColorEditor : ExtendedPropertyValueEditor
    {
        public LightColorEditor()
        {
            this.InlineEditorTemplate = Application.Current.FindResource("LightColorTemplate") as DataTemplate;
        }
    }
}
