using System.Activities.Presentation.PropertyEditing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Delight.Component.PropertyEditor
{
    public class PercentageEditor : ExtendedPropertyValueEditor
    {
        public PercentageEditor()
        {
            this.InlineEditorTemplate = Application.Current.FindResource("PercentageTemplate") as DataTemplate;
        }
    }
}
