using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component
{
    public class DelightTheme : ResourceDictionary
    {
        public DelightTheme()
        {
            AddDictionary("pack://application:,,,/Delight.Component;component/Controls/ControlThemes.xaml");
            AddDictionary("pack://application:,,,/Delight.Component;component/Layers/LayerTheme.xaml");
            AddDictionary("pack://application:,,,/Delight.Component;component/Primitives/MediaElementPro.xaml");
            AddDictionary("pack://application:,,,/Delight.Component;component/PropertyEditor/EditorResources.xaml");
        }

        public ResourceDictionary GetDictionary(string uriString)
        {
            return new ResourceDictionary()
            {
                Source = new Uri(uriString)
            };
        }

        public void AddDictionary(string uriString)
        {
            MergedDictionaries.Add(GetDictionary(uriString));
        }
    }
}
