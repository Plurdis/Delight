using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Delight.Controls.Property
{
    [TemplatePart(Name = "slider", Type = typeof(PrestoSeekBar))]
    [TemplatePart(Name = "runPropName", Type = typeof(Run))]
    [TemplatePart(Name = "runValue", Type = typeof(Run))]
    public class PropertySlider : Control, INotifyPropertyChanged
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PropertySlider));
        
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        double _savedValue = double.MinValue,
               _savedMaximum = double.MinValue,
               _savedMinimum = double.MinValue;
        bool _savedIsTracking = true;

        public bool IsTracking
        {
            get => (bool)slider.GetValue(PrestoSeekBar.IsTrackingProperty);
            set
            {
                if (slider == null)
                {
                    _savedIsTracking = value;
                }
                else
                {
                    slider.SetValue(PrestoSeekBar.IsTrackingProperty, value);
                }
                
            }
        }

        public double Maximum
        {
            get => (double)slider.GetValue(PrestoSeekBar.MaximumProperty);
            set
            {
                if (slider == null)
                    _savedMaximum = value;
                else
                    slider.SetValue(PrestoSeekBar.MaximumProperty, value);
            }
        }

        public double Minimum
        {
            get => (double)slider.GetValue(PrestoSeekBar.MinimumProperty);
            set
            {
                if (slider == null)
                    _savedMinimum = value;
                else
                    slider.SetValue(PrestoSeekBar.MinimumProperty, value);
            }
        }

        public double Value
        {
            get => (double)slider.GetValue(PrestoSeekBar.ValueProperty);
            set
            {
                if (slider == null)
                    _savedValue = value;
                else
                    slider.SetValue(PrestoSeekBar.ValueProperty, value);
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        PrestoSeekBar slider;
        Run runPropName, runValue;

        public PropertySlider()
        {
            ApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            runPropName = GetTemplateChild("runPropName") as Run;
            runValue = GetTemplateChild("runValue") as Run;
            slider = GetTemplateChild("slider") as PrestoSeekBar;
            
            slider.ValueChanged += (s, e) =>
            {
                runValue.Text = ((int)((Value / Maximum) * 100)).ToString();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            };

            if (_savedValue != double.MinValue)
            {
                Value = _savedValue;
            }

            if (_savedMaximum != double.MinValue)
            {
                Maximum = _savedMaximum;
            }

            if (_savedMinimum != double.MinValue)
            {
                Minimum = _savedMinimum;
            }

            IsTracking = _savedIsTracking;

        }
    }
}
