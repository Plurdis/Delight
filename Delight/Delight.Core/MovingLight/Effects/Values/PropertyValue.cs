using Delight.Component.MovingLight.Effects.Values.Base;

namespace Delight.Component.MovingLight.Effects.Values
{
    public class PropertyValue : BaseValue
    {
        public PropertyValue()
        {

        }
        public PropertyValue(string propertyName)
        {
            PropertyName = propertyName;
        }
        public string PropertyName { get; set; }

        public static implicit operator PropertyValue(string s)  // explicit byte to digit conversion operator
        {
            PropertyValue v = new PropertyValue(s);

            return v;
        }
    }
}
