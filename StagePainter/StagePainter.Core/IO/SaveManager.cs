using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StagePainter.Core.Attributes;

namespace StagePainter.Core.IO
{
    /// <summary>
    /// manager that can organize and save data
    /// </summary>
    public class SaveManager
    {
        
    }

    /// <summary>
    /// SaveManager's Extension Class
    /// </summary>
    public static class SaveManagerEx
    {
        /// <summary>
        /// Save single data as file
        /// </summary>
        /// <param name="data">Data that save to file that have <see cref="SerializableAttribute"/> Attribute</param>
        /// <param name="fileLocation">Input where you save file.</param>
        /// <returns></returns>
        public static bool SaveFile(this object data, string fileLocation)
        {
            // Judge data is 'Struct Based' or 'Class Based'
            if (data.GetType().IsValueType)
            {
                // Struct Based
                Console.WriteLine("Value Type");
            }
            else
            {
                // Class Based
                Console.WriteLine("Struct Based");
            }


            System.IO.File.WriteAllBytes(fileLocation, null);

            return true;
        }
    }
}
