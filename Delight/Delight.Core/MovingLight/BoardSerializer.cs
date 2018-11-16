using Delight.Core.Common;
using Newtonsoft.Json;
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
        // 저장을 Xml에서 Json으로 변경
        public static void Save(SetterBoard group, string filePath)
        {
            JsonSerializer serializer = new JsonSerializer();
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    serializer.Serialize(writer, group);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string SerializeToString(SetterBoard group)
        {
            JsonSerializer serializer = new JsonSerializer();
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
            JsonSerializer serializer = new JsonSerializer();
            try
            {
                using (StringReader reader = new StringReader(data))
                {
                    return serializer.Deserialize(reader, typeof(SetterBoard)) as SetterBoard;
                }
            }
            catch (Exception)
            {
                XmlSerializer serializer2 = new XmlSerializer(typeof(SetterBoard));
                try
                {
                    using (StringReader reader = new StringReader(data))
                    {
                        return serializer2.Deserialize(reader) as SetterBoard;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SetterBoard Load(string filePath)
        {
            JsonSerializer serializer = new JsonSerializer();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return serializer.Deserialize(reader, typeof(SetterBoard)) as SetterBoard;
                }
            }
            catch (Exception)
            {
                XmlSerializer serializer2 = new XmlSerializer(typeof(SetterBoard));
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        return serializer2.Deserialize(reader) as SetterBoard;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            
        }
    }
}
