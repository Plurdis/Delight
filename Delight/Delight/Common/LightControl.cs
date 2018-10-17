using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Common
{
    public static class LightControl
    {

        public static void Control(params (int,int)[] values)
        {
            //int[] values = new int[] {
            //    (int)slX.Value,
            //    (int)slY.Value,
            //    (int)slLight.Value,
            //    (int)slTicking.Value };

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "LightController.exe";
            startInfo.Arguments = ConvertToArguments(values);
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process processTemp = new Process();
            processTemp.StartInfo = startInfo;
            processTemp.EnableRaisingEvents = true;
            try
            {
                processTemp.Start();
            }
            catch (Exception e)
            {
            }
        }

        public static string ConvertToArguments(IEnumerable<(int,int)> values)
        {
            var builder = new StringBuilder();
            foreach((int,int) value in values)
            {
                builder.Append($"{value.Item1}:{value.Item2} ");
            }

            return builder.ToString();
        }
    }
}
