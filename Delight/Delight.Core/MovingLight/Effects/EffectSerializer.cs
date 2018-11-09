using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Delight.Core.MovingLight.Effects
{
    public static class EffectSerializer
    {
        public static bool WriteEffect(LightBoard board)
        {
            return true;
        }

        public static LightBoard GetStatesFromFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LightBoard));

            using (StreamReader sr = new StreamReader(filePath))
            {
                var items = serializer.Deserialize(sr);
                Console.WriteLine(items.GetType().ToString());

                return items as LightBoard;
            }
        }

        public static void SaveSourcesFromList(LightBoard lightBoard, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LightBoard));
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    serializer.Serialize(writer, lightBoard);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
        }
    }
}
