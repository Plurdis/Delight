using Delight.Component.MovingLight.Effects.Values.Base;

namespace Delight.Component.MovingLight.Effects.Values
{
    public class RelativeValue : BaseValue
    {
        public enum RelativeSign
        {
            Plus,
            Minus,
        }

        public RelativeValue(string propertyName, byte relativeValue, RelativeSign sign)
        {
            PropertyName = propertyName;
            Value = relativeValue;
            Sign = sign;
        }

        public string PropertyName { get; set; }

        public byte Value { get; set; }

        public RelativeSign Sign { get; set; }
    }
}
