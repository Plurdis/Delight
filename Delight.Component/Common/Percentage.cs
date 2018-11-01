using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Common
{
    public class Percentage
    {
        public Percentage(double value, double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
            Value = value;
        }

        public Percentage(double value)
        {
            Minimum = double.MinValue;
            Maximum = double.MaxValue;
            Value = value;
        }

        public double Minimum { get; set; }

        public double Maximum { get; set; }

        double _value;
        public double Value
        {
            get => _value;
            set
            {
                if (value > Maximum)
                    _value = Maximum;
                else if (value < Minimum)
                    _value = Minimum;
                else
                    _value = value;
            }
        }

        public override string ToString()
        {
            return Value * 100 + "%";
        }


        public static implicit operator Percentage(double d)
        {
            return new Percentage(d);
        }

        public static implicit operator double(Percentage p)
        {
            return p.Value;
        }
    }
}
