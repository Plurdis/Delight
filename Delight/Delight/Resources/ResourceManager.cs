using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Resources
{
    public static class ResourceManager
    {
        #region [ Resource ]
        public static Stream GetStreamResource(string path)
        {
            string uri = BuildResourceUri(path);


            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(uri);
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string GetTextResource(string path)
        {
            Stream s = GetStreamResource(path);

            using (var sr = new StreamReader(s)) return sr.ReadToEnd();
        }

        private static string BuildResourceUri(string path)
        {
            return $"Delight.Resources.{path.Replace('/', '.')}";
        }
        #endregion
    }
}
