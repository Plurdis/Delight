using Delight.Core.Common;
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

                using (StringWriter writer = new StringWriterWithEncoding())
                {
                    serializer.Serialize(writer, lightBoard);

                    string str = writer.ToString();

                    string hash = Crc32.GetHashFromString(str);
                    string hash2 = Crc32.GetHashFromFile(filePath);

                    Console.WriteLine($"hash : {hash} | hash2 : {hash2} | Is Same? : {hash == hash2}");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
        }

        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding() : this(Encoding.UTF8) { }

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }
    }
}
