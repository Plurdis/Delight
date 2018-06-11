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
        /// Save data as file
        /// </summary>
        /// <typeparam name="T">Generic Type for data</typeparam>
        /// <param name="data">data that save to file</param>
        /// <param name="fileLocation">input where you save file.</param>
        /// <returns></returns>
        public static bool SaveFile<T>(T data, string fileLocation)
        {
            // Judge data is 'Struct Based' or 'Class Based'

            if (data is object)
            {

            }

            System.IO.File.WriteAllBytes()
        }
    }
}
