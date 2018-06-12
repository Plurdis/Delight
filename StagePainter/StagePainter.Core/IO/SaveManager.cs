using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns></returns>
        public static bool SaveFile<T>(this object data, string fileLocation) where T : class
        {
            System.IO.File.WriteAllBytes(fileLocation, null);

            return true;
        }
    }
}
