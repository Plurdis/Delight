using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.Extension
{
    /// <summary>
    /// <see cref="object"/>의 확장 함수들을 모아놓은 클래스입니다.
    /// </summary>
    public static class ObjectEx
    {
        /// <summary>
        /// 현재 <see cref="object"/>의 <see cref="Attribute"/>를 가져옵니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this object obj) where T : Attribute
        {
            Type type = obj.GetType();

            var itm = type.GetCustomAttributes(true);

            return type.GetCustomAttributes(true)
                       .Where(i => i is T)
                       .FirstOrDefault() as T;
        }

        /// <summary>
        /// 현재 <see cref="Enum"/>의 <see cref="Attribute"/>를 가져옵니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static T GetEnumAttribute<T>(this Enum enumValue) where T : Attribute
        {
            FieldInfo type = enumValue.GetType().GetField(enumValue.ToString());

            var itm = type.GetCustomAttributes(true);

            return type.GetCustomAttributes(true)
                       .Where(i => i is T)
                       .FirstOrDefault() as T;
        }

        /// <summary>
        /// <see cref="object"/>가 해당 Attribute를 가졌는지를 확인합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this object obj) where T : Attribute
        {
            return (obj.GetAttribute<T>() == null);
        }
    }
}
