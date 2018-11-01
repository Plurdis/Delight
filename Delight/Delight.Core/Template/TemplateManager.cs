using Delight.Core.Template.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Delight.Core.Template
{
    public static class TemplateManager
    {
        public static List<BaseSource> GetSourcesFromFile(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);

            XmlSerializer serializer = new XmlSerializer(typeof(BaseSource));
            var items = serializer.Deserialize(sr);

            Console.WriteLine(items.GetType().ToString());

            return null;
        }

        public static void SaveSourcesFromList(ExternalSources items, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ExternalSources));
            try
            {
                serializer.Serialize(new StreamWriter(filePath, false), items);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
        }
    }
}
