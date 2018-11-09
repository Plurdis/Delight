using Delight.Core.Extensions;
using Delight.Core.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Delight.Core.Template
{
    public static class FileCacheDictionary
    {
        static FileCacheDictionary()
        {
            SyncDictionary();
        }

        private static Dictionary<string, (string,string)> FileDict;

        public static string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Delight\\" + "fileDictionary.xml";

        public static string GetFileNameFromTitle(YoutubeSource source)
        {
            SyncDictionary();
            return FileDict[source.Title].Item1;
        }

        public static string GetIdFromTitle(YoutubeSource source)
        {
            SyncDictionary();
            return FileDict[source.Title].Item2;
        }

        public static void AddPair(string title, string fileName, string id)
        {
            FileDict.Add(title, (fileName,id));
            SyncToFile();
        }

        public static KeyValuePair<string,(string,string)> GetPathFromId(string id)
        {
            foreach (var item in FileDict)
            {
                string fId = item.Value.Item2;

                if (id == fId)
                    return item;
            }

            return default(KeyValuePair<string,(string,string)>);
        }

        public static bool ContainsTitle(string title)
        {
            return FileDict.ContainsKey(title);
        }

        public static bool ContainsId(string id)
        {
            bool flag = false;

            FileDict.ForEach(i =>
            {
                string fId = i.Value.Item2;

                if (id == fId)
                    flag = true;
            });

            return flag;
        }

        private static void SyncToFile()
        {
            IEnumerable<XElement> obj = FileDict.Select(i => new XElement("Item", 
                new XElement("Title", i.Key), 
                new XElement("FileName", (new FileInfo(i.Value.Item1).Name)),
                new XElement("Id", i.Value.Item2)));

            XElement root = new XElement("Root",obj);

            root.Save(FilePath);
        }

        private static void SyncDictionary()
        {
            if (!File.Exists(FilePath))
            {
                FileDict = new Dictionary<string, (string,string)>();
                return;
            }

            XElement root = XElement.Load(FilePath);

            var dict = new Dictionary<string, (string, string)>();
            foreach (XElement el in root.Elements())
            {
                string title = el.Element("Title").Value;
                string fileName = el.Element("FileName").Value;
                string Id = el.Element("Id").Value;
                dict.Add(title, (fileName, Id));
            }
            
            FileDict = dict;
        }
    }
}
