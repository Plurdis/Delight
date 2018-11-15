using Delight.Component.Controls;
using Delight.Component.MovingLight.Effects.Setters;
using Delight.Component.MovingLight.Effects.Values.Base;
using Delight.Core.Common;
using Delight.Core.Extensions;
using Delight.Core.MovingLight;
using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.MovingLight.Effects
{
    public class SetterBoard
    {
        public SetterBoard()
        {
            SetterGroups = new List<SetterGroup>();
            SetterProperties = new List<SetterProperty>();
        }

        public string Identifier { get; set; }

        public List<SetterGroup> SetterGroups { get; set; }

        public List<SetterProperty> SetterProperties { get; set; }

        public void AddSetterGroup()
        {
            SetterGroups.Add(new SetterGroup());
        }


        public List<ValueSetter> InitalizeValue { get; set; }

        public void SetInitalizeValue(params (PortNumber, BaseValue)[] values)
        {
            InitalizeValue = new List<ValueSetter>(
                values.Select(i => new ValueSetter()
                {
                    Port = i.Item1,
                    Value = i.Item2,
                }));
        }


        public void AddSetterProperties(PortNumber portNumber, string propName, string displayName)
        {
            SetterProperties.Add(new SetterProperty(portNumber, propName, displayName));
        }

        //public DynamicProperty GetDynamicProperty()
        //{
        //    DynamicProperty property = new DynamicProperty();

        //    Dictionary<string, PortNumber> valueDict = new Dictionary<string, PortNumber>();
        //    ((dynamic)property).ValueDictionary = valueDict;

        //    foreach (var prop in SetterProperties)
        //    {
        //        valueDict.Add(prop.PropertyName, prop.PortNumber);
        //        PropertyManager.SetProperty(property,prop.PropertyName, 0);
        //    }

        //    return property;
        //}

        public BaseLightSetterProperty GetSetterBaseProperty()
        {
            var type = typeof(BaseLightSetterProperty);

            BaseLightSetterProperty property = new BaseLightSetterProperty();

            var aName = new AssemblyName("DynamicProperties");
            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule(aName.Name);
            var tb = mb.DefineType(Identifier, TypeAttributes.Public, type);

            foreach (var prop in SetterProperties)
            {
                var fb = tb.DefineField($"_{prop.PropertyName.ToLower()}", typeof(byte), FieldAttributes.Private);
                var pb = tb.DefineProperty(prop.PropertyName, PropertyAttributes.HasDefault, typeof(byte), null);

                MethodBuilder setter = tb.DefineMethod($"set_{prop.PropertyName}", MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { typeof(byte) });
                ILGenerator setterILG = setter.GetILGenerator();
                setterILG.Emit(OpCodes.Ldarg_0);
                setterILG.Emit(OpCodes.Ldarg_1);
                setterILG.Emit(OpCodes.Stfld, fb);
                setterILG.Emit(OpCodes.Ret);
                pb.SetSetMethod(setter);

                //build getter
                MethodBuilder getter = tb.DefineMethod($"get_{prop.PropertyName}", MethodAttributes.Public | MethodAttributes.Virtual, typeof(byte), Type.EmptyTypes);
                ILGenerator getterILG = getter.GetILGenerator();
                getterILG.Emit(OpCodes.Ldarg_0);
                getterILG.Emit(OpCodes.Ldfld, fb);
                getterILG.Emit(OpCodes.Ret);
                pb.SetGetMethod(getter);

                Type[] attrCtorParams = new Type[] { typeof(string) };
                ConstructorInfo attrCtorInfo = typeof(DisplayNameAttribute).GetConstructor(attrCtorParams);
                CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(attrCtorInfo, new object[] { prop.DisplayName });
                pb.SetCustomAttribute(attrBuilder);

                Type[] parameter = new Type[] { };
                ConstructorInfo constInfo = typeof(DesignElementAttribute).GetConstructor(parameter);

                string editorKey = string.Empty;

                if (prop.PortNumber == PortNumber.Blink ||
                    prop.PortNumber == PortNumber.XAxis ||
                    prop.PortNumber == PortNumber.YAxis ||
                    prop.PortNumber == PortNumber.Brightness)
                    editorKey = "BytePercentage";
                else if (prop.PortNumber == PortNumber.Color)
                    editorKey = "LightColor";

                PropertyInfo nameProp = typeof(DesignElementAttribute).GetRuntimeProperty("DisplayName");
                PropertyInfo keyProp = typeof(DesignElementAttribute).GetRuntimeProperty("Key");
                PropertyInfo categoryProp = typeof(DesignElementAttribute).GetRuntimeProperty("Category");

                CustomAttributeBuilder attrBuilder2 = 
                    new CustomAttributeBuilder(constInfo, new object[] { }, 
                        new PropertyInfo[] { nameProp, keyProp, categoryProp }, 
                        new object[] { prop.DisplayName, editorKey, "효과 속성" } );
                pb.SetCustomAttribute(attrBuilder2);
            }

            var newType = tb.CreateType();
            var instance = (BaseLightSetterProperty)Activator.CreateInstance(newType);

            return instance;
        }

        public SetterGroup this[int index] => SetterGroups[index]; 
    }
}
