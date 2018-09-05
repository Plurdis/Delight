using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace Delight.Core.Template.Common
{
    public static class ZipManager
    {
        // TODO: 함수 완성
        public static DelightTemplate GetDelightTemplate(string fileName)
        {
            ZipArchive archive = ZipFile.OpenRead(fileName);
            var entries = archive.GetEntry("template.list");

            Stream stream = entries.Open();
            StreamReader stReader = new StreamReader(stream);

            Console.WriteLine(stReader.ReadToEnd());

            return null;
        }
    }

}
