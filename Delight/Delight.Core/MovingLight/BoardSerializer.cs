﻿using Delight.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Delight.Component.MovingLight.Effects
{
    public class BoardSerializer
    {
        public static void Save(SetterBoard group, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SetterBoard));
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    serializer.Serialize(writer, group);
                }

                //using (StringWriter writer = new StringWriterWithEncoding())
                //{
                //    serializer.Serialize(writer, lightBoard);

                //    string str = writer.ToString();

                //    string hash = Crc32.GetHashFromString(str);
                //    string hash2 = Crc32.GetHashFromFile(filePath);

                //    Console.WriteLine($"hash : {hash} | hash2 : {hash2} | Is Same? : {hash == hash2}");
                //}
            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine("Error");
            }
        }

        internal static string SerializeToString(SetterBoard group)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SetterBoard));
            try
            {
                StringBuilder sb = new StringBuilder();
                using (EncodingStringWriter writer = new EncodingStringWriter(sb))
                {
                    serializer.Serialize(writer, group);
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SetterBoard LoadFromString(string data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SetterBoard));
            try
            {
                using (StringReader reader = new StringReader(data))
                {
                    return serializer.Deserialize(reader) as SetterBoard;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static SetterBoard Load(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SetterBoard));
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return serializer.Deserialize(reader) as SetterBoard;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
