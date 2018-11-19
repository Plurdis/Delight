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
        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        // 저장을 Xml에서 Json으로 변경
        public static void Save(SetterBoard group, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {


                    writer.Write(JsonConvert.SerializeObject(group, Formatting.Indented, settings));
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
                    writer.Write(JsonConvert.SerializeObject(group, Formatting.Indented,settings));
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
            try
            {
                using (StringReader reader = new StringReader(data))
                {
                    return JsonConvert.DeserializeObject<SetterBoard>(reader.ReadToEnd(), settings);
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
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return JsonConvert.DeserializeObject<SetterBoard>(reader.ReadToEnd(), settings);
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
