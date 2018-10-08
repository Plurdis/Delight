using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Delight.Controls
{
    [TemplatePart(Name = "Items", Type = typeof(StackPanel))]
    [TemplatePart(Name = "gridHeader", Type = typeof(Grid))]
    public class PresetItem : Control
    {
        public PresetItem()
        {
            this.Style = FindResource("PresetItemStyle") as Style;

            ApplyTemplate();

            _items = GetTemplateChild("Items") as StackPanel;
            _gridHeader = GetTemplateChild("gridHeader") as Grid;


            _gridHeader.MouseDown += (s, e) =>
            {
                Checked = !Checked;
            };
        }

        #region [  Variables  ]

        StackPanel _items;
        Grid _gridHeader;

        #endregion

        #region [  DependencyProperties  ]

        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PresetItem), new PropertyMetadata("Preset"));

        public static DependencyProperty CheckedProperty = DependencyProperty.Register(nameof(Checked), typeof(bool), typeof(PresetItem));

        #endregion

        #region [  Properties  ]

        public string Text
        {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }

        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty);
            set => SetValue(CheckedProperty, value);
        }

        #endregion

        public void UpdateItem()
        {
            PresetPanel panel = (((WrapPanel)this.Parent).TemplatedParent as PresetPanel);
            int count = panel.TrackTypes.Count();

            if (count < _items.Children.Count)
            {
                int diff = _items.Children.Count - count;
            }
            else if (count > _items.Children.Count)
            {
                int diff = count - _items.Children.Count;
            }

        }
    }
}
