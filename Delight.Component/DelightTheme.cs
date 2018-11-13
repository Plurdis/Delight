using System;
using System.Text;
using System.Windows;

namespace Delight.Component
{
    public class DelightTheme : ResourceDictionary
    {
        public DelightTheme()
        {
            AddDictionary("Resources/Colors.xaml");
            AddDictionary("Resources/Paths.xaml");

            AddDictionary("Controls/CircleBar.xaml");
            AddDictionary("Controls/TrackItem.xaml");
            AddDictionary("Controls/TimeLineScrollBar.xaml");
            AddDictionary("Controls/TimeLine.xaml");
            AddDictionary("Controls/Track.xaml");
            AddDictionary("Controls/PropertyGrid.xaml");

            AddDictionary("Layers/LayerTheme.xaml");

            AddDictionary("Primitives/MediaElementPro.xaml");

            AddDictionary("SetterStyles.xaml");
        }

        ResourceDictionary GetDictionary(string uriString)
        {
            return new ResourceDictionary()
            {
                Source = new Uri($"pack://application:,,,/Delight.Component;component/{uriString}")
            };
        }

        void AddDictionary(string uriString)
        {
            MergedDictionaries.Add(GetDictionary(uriString));
        }
    }
}
