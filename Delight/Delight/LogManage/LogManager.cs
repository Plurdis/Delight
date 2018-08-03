using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.LogManage
{
    public static class LogManager
    {
        static string logFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Delight_runningLog.log");

        public delegate void TextChangedDelegate(string text);

        public delegate void ProgressChangedDelegate(double value);

        public static event TextChangedDelegate InfoTextChanged;

        public static event ProgressChangedDelegate InfoProgressChanged;

        public static event ProgressChangedDelegate InfoMaximumChanged;

        public static void Log(this string text)
        {
            WriteLine(text);
        }

        public static void WriteLine(string text)
        {
            InfoTextChanged?.Invoke(text);
        }

        public static void SetProgressValue(double value)
        {
            InfoProgressChanged?.Invoke(value);
        }

        public static void SetMaximumValue(double value)
        {
            InfoMaximumChanged?.Invoke(value);
        }

        public static bool SetLogFile(string fileLocation)
        {
            if (File.Exists(fileLocation))
            {
                logFileLocation = fileLocation;
                return true;
            }

            return false;
        }
    }
}
