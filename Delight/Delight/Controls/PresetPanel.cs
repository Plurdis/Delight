using Delight.Core.Extension;
using Delight.Core.Extensions;
using Delight.Extensions;
using Delight.Timing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Delight.Controls
{
    [TemplatePart(Name = "spTypes", Type = typeof(StackPanel))]
    [TemplatePart(Name = "presetItems", Type = typeof(WrapPanel))]
    public class PresetPanel : Control
    {
        public PresetPanel()
        {
            this.Style = FindResource("PresetPanelStyle") as Style;
            
            Presets = new ObservableCollection<PresetItem>();
            Presets.CollectionChanged += Presets_CollectionChanged;

            ApplyTemplate();

            spTypes = GetTemplateChild("spTypes") as StackPanel;
            presetItems = GetTemplateChild("presetItems") as WrapPanel;


            presetItems.Children.Add(new PresetItem()
            {
                Text = "테스트1",
                Width = 200
            });

            presetItems.Children.Add(new PresetItem()
            {
                Text = "테스트2",
                Width = 200
            });

            presetItems.Children.Add(new PresetItem()
            {
                Text = "테스트3",
                Width = 200
            });


            AddType(TrackType.Image);
            AddType(TrackType.Unity);
            AddType(TrackType.Video);
            AddType(TrackType.Sound);
            AddType(TrackType.Light);
            
            //var tag = ((Label)spTypes.Children[0]).Tag;
        }

        #region [  Variables  ]

        StackPanel spTypes;
        WrapPanel presetItems;

        #endregion

        #region [  Properties  ]

        public ReadOnlyCollection<TrackType> TrackTypes => spTypes.Children
            .Cast<Label>()
            .Select(i => i.GetTag<TrackType>())
            .ToList()
            .AsReadOnly();

        ObservableCollection<PresetItem> Presets { get; }

        int SelectedIndex { get; set; } = -1;
        PresetItem SelectedItem
        {
            get => Presets[SelectedIndex];
            set => SelectedIndex = Presets.IndexOf(value);
        }

        #endregion

        public void AddType(TrackType type)
        {
            spTypes.Children.Add(new Label()
            {
                Content = type.GetEnumAttribute<DescriptionAttribute>().Description,
                Tag = type,
            });

            presetItems.Children.Cast<PresetItem>().ForEach(i => i.UpdateItem());
        }

        private void Presets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // 0 1
                if (SelectedIndex + 1 > Presets.Count)
                {
                    SelectedIndex = Presets.Count - 1;
                }
            }
        }
        
        public void Play()
        {
            Play(SelectedIndex);
        }

        public void Play(int index)
        {

        }
    }
}
