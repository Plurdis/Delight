using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight
{
    public class DynamicProperty : DynamicObject, INotifyPropertyChanged, ICustomTypeDescriptor
    {
        private readonly Dictionary<string, object> _dynamicProperties =
            new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var memberName = binder.Name;
            return _dynamicProperties.TryGetValue(memberName, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var memberName = binder.Name;
            _dynamicProperties[memberName] = value;
            NotifyToRefreshAllProperties();
            return true;
        }


        #region Implementation of ICustomTypeDescriptor

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            IEnumerable<DynamicPropertyDescriptor> properties = _dynamicProperties
                .Select(pair => new DynamicPropertyDescriptor(this,
                    pair.Key, pair.Value.GetType(), attributes));
            List<DynamicPropertyDescriptor> list = new List<DynamicPropertyDescriptor>();
            foreach (DynamicPropertyDescriptor property in properties)
                list.Add(property);
            return new PropertyDescriptorCollection(list.ToArray());
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public string GetClassName()
        {
            return GetType().Name;
        }
        #endregion

        #region Hide not implemented members

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, eventArgs);
        }

        private void NotifyToRefreshAllProperties()
        {
            OnPropertyChanged(string.Empty);
        }

        #endregion

        private class DynamicPropertyDescriptor : PropertyDescriptor
        {
            private readonly DynamicProperty _dynamicProperty;
            private readonly Type _propertyType;

            public DynamicPropertyDescriptor(DynamicProperty dynamicProperty,
                string propertyName, Type propertyType, Attribute[] propertyAttributes)
                : base(propertyName, propertyAttributes)
            {
                _dynamicProperty = dynamicProperty;
                _propertyType = propertyType;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override object GetValue(object component)
            {
                return _dynamicProperty._dynamicProperties[Name];
            }

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value)
            {
                _dynamicProperty._dynamicProperties[Name] = value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get { return typeof(DynamicProperty); }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return _propertyType; }
            }
        }
    }
}