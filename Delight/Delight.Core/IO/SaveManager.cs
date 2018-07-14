using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Delight.Core.Exceptions;
using Delight.Core.Extension;

namespace Delight.Core.IO
{
    /// <summary>
    /// 데이터를 구성하고 저장할 수 있는 저장 관리자 입니다.
    /// </summary>
    public static class SaveManager
    {
        /// <summary>
        /// 하나의 단일 데이터를 저장합니다.
        /// </summary>
        /// <param name="data"><see cref="SerializableAttribute"/> 특성을 가지고 있는 저장할 데이터입니다.</param>
        /// <param name="fileLocation">저장할 파일의 위치를 선택합니다.</param>
        /// <exception cref="AttributeNotFoundException"/>
        /// <exception cref="SaveFailException"/>
        /// <returns></returns>
        public static bool SaveData<T>(this object data, string fileLocation) where T : class
        {
            var attr = data.GetAttribute<StorageMethodAttribute>();

            if (attr == null)
                throw new AttributeNotFoundException();

            string formattedText = FormatText(attr.StorageMethodType);

            try
            {
                System.IO.File.WriteAllBytes(fileLocation, attr.EncodingType.GetBytes(formattedText));
            }
            catch (Exception ex)
            {
                throw new SaveFailException("파일을 저장할 수 없습니다. 자세한 사항은 내부 예외를 참조하세요.", ex);
            }

            return true;
        }

        public static string FormatText(StorageMethodTypes type)
        {
            switch (type)
            {
                case StorageMethodTypes.XML:
                    break;
                case StorageMethodTypes.Json:
                    break;
                case StorageMethodTypes.Serialize:
                    break;
                case StorageMethodTypes.Custom:
                    break;
                default:
                    break;
            }

            return string.Empty;
        }
    }
}
