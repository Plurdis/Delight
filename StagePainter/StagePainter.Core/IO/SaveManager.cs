using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StagePainter.Core.Exceptions;
using StagePainter.Core.Extension;

namespace StagePainter.Core.IO
{
    /// <summary>
    /// manager that can organize and save data
    /// </summary>
    public static class SaveManager
    {
        /// <summary>
        /// Save single data as file
        /// </summary>
        /// <param name="data">Data that save to file that have <see cref="SerializableAttribute"/> Attribute</param>
        /// <param name="fileLocation">Input where you save file.</param>
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
                throw new SaveFailException("Can't save file. See InnerException for details.", ex);
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
