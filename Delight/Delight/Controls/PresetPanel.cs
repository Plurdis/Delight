using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Delight.Controls
{
    public class PresetPanel : Control
    {
        public PresetPanel()
        {
            this.Style = FindResource("PresetPanelStyle") as Style;
            
            Presets = new ObservableCollection<PresetItem>();
            Presets.CollectionChanged += Presets_CollectionChanged;
        }

        #region [  Properties  ]

        ObservableCollection<PresetItem> Presets { get; }

        int SelectedIndex { get; set; } = -1;
        PresetItem SelectedItem
        {
            get => Presets[SelectedIndex];
            set
            {
                SelectedIndex = Presets.IndexOf(value);
            }
        }

        #endregion

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
