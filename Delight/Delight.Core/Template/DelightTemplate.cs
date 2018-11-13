﻿using Delight.Component.Common;
using Delight.Component.MovingLight.Effects;
using Delight.Core.Sources;
using Delight.Core.Stage.Components;
using Delight.Core.Stage.Components.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Delight.Core.Template
{

    public class DelightTemplate
    {
        public string Name { get; set; }

        public List<BaseSource> Sources { get; set; }

        public List<ItemPosition> DeployingPositions { get; set; }

        public void Pack(string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DelightTemplate));
            
            using (FileStream zipToOpen = new FileStream(@"C:\Users\장유탁\Desktop\DelightPackage.dpack", FileMode.CreateNew))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var fileList = new StringBuilder();
                    foreach (BaseSource source in Sources)
                    {
                        TemplateData data = source.GetTemplateData();

                        fileList.AppendLine($"{{{source.TypeText}}} {{{data.Id}}} {{{data.StreamUse}}} {{{data.FileName}}}");

                        if (data.StreamUse)
                        {
                            ZipArchiveEntry readmeEntry = archive.CreateEntry($"{data.FileName}");
                            CopyStream(data.Stream, readmeEntry.Open());
                        }
                    }

                    ZipArchiveEntry entry = archive.CreateEntry("FileDictionary");
                    using (StreamWriter sw = new StreamWriter(entry.Open()))
                    {
                        sw.Write(fileList);
                        sw.Flush();
                    }


                    entry = archive.CreateEntry("DeployingPosition");
                    using (StreamWriter sw = new StreamWriter(entry.Open()))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<ItemPosition>));
                        serializer.Serialize(sw, DeployingPositions);
                    }
                }
            }
        }

        public static string DelightAppPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Delight");

        public static string TempDelightTemplate => Path.Combine(DelightAppPath, "Template Extracted");

        public static DelightTemplate FromFile(string path)
        {
            var template = new DelightTemplate();
            var baseSources = new List<BaseSource>();

            ZipFile.ExtractToDirectory(path, TempDelightTemplate);

            FileInfo[] files = new DirectoryInfo(TempDelightTemplate).GetFiles();

            foreach (FileInfo itm in files)
            {
                if (itm.Name == "DeployingPosition")
                {
                    StreamReader sr = new StreamReader(File.Open(itm.FullName, FileMode.Open));
                    string data = sr.ReadToEnd();
                    XmlSerializer serializer = new XmlSerializer(typeof(List<ItemPosition>));
                    var positions = (List<ItemPosition>)serializer.Deserialize(new StringReader(data));

                    template.DeployingPositions = positions;
                }
                else if (itm.Name == "FileDictionary")
                {
                    StreamReader sr = new StreamReader(File.Open(itm.FullName, FileMode.Open));
                    string data = sr.ReadToEnd();
                    string pattern = "{(.+)} {(.+)} {(.+)} {(.+)}";
                    foreach (Match m in Regex.Matches(data, pattern))
                    {                                
                        string type = m.Groups[1].Value;
                        string id = m.Groups[2].Value;
                        string useSource = m.Groups[3].Value;
                        string fileName = m.Groups[4].Value;

                        Console.WriteLine($"{type} {id} {useSource} {fileName}");

                        BaseSource source = null;
                        Stream st;
                        switch (type)
                        {
                            case "외부 영상":
                                st = File.Open(files.Where(i => i.Name == fileName).FirstOrDefault().FullName, FileMode.Open);
                                string currentPath = DelightAppPath + @"\" + fileName;

                                using (var fileStream = File.Create(currentPath))
                                {
                                    st.Seek(0, SeekOrigin.Begin);
                                    st.CopyTo(fileStream);
                                }

                                source = new ExternalVideoSource()
                                {
                                    FullPath = currentPath,
                                    Title = fileName,
                                    Id = id,
                                };

                                break;
                            case "유튜브 영상":
                                source = YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=" + id);
                                break;
                            case "조명 아이템":
                                st = File.Open(files.Where(i => i.Name == fileName).FirstOrDefault().FullName, FileMode.Open);

                                string lightCurrentPath = DelightAppPath + @"\" + fileName;

                                using (var fileStream = File.Create(lightCurrentPath))
                                {
                                    st.Seek(0, SeekOrigin.Begin);
                                    st.CopyTo(fileStream);
                                }

                                source = new LightSource()
                                {
                                    Title = fileName,
                                    Id = id,
                                    MovingData = new StreamReader(lightCurrentPath).ReadToEnd(),
                                };
                                break;
                            default:
                                break;
                        }

                        if (source != null)
                            baseSources.Add(source);
                    }
                }
            }

            template.Sources = baseSources;

            return template;
        }

        public static StageComponent ConvertToComponent(BaseSource source)
        {
            if (source is ExternalVideoSource externalVideoSource)
            {
                return new VideoMedia()
                {
                    FromYoutube = false,
                    Id = source.Id,
                    Path = externalVideoSource.FullPath,
                    Time = MediaTools.GetMediaDuration(externalVideoSource.FullPath),
                    Thumbnail = ((BitmapImage)MediaTools.GetMediaThumbnail(externalVideoSource.FullPath)).UriSource,
                    Identifier = externalVideoSource.Title,
                };
            }
            else if (source is YoutubeSource youtubeSource)
            {
                source.Download(0);

                var item = FileCacheDictionary.GetPathFromId(source.Id);

                string path = Path.Combine(DelightAppPath, item.Value.Item1);

                return (new VideoMedia()
                {
                    Identifier = item.Key,
                    Time = MediaTools.GetMediaDuration(path),
                    Path = path,
                    Thumbnail = new Uri(source.ThumbnailUri),
                    FromYoutube = true,
                    DownloadID = source.Id,
                    Id = source.Id,
                });
            }
            else if (source is LightSource lightSource)
            {
                return new LightComponent(BoardSerializer.LoadFromString(lightSource.MovingData));
            }

            return null;
        }

        public static BaseSource ConvertToSource(StageComponent comp)
        {
            if (comp is VideoMedia media)
            {
                if (media.FromYoutube)
                {
                    return new YoutubeSource(media.Identifier, media.Thumbnail.OriginalString, media.DownloadID);
                }
                else
                {
                    return new ExternalVideoSource()
                    {
                        FullPath = media.Path,
                        Id = media.Id,
                        ThumbnailUri = media.Thumbnail.OriginalString,
                        Title = media.Identifier,
                    };
                }
            }
            else if (comp is LightComponent lightComponent)
            {
                return new LightSource()
                {
                    Id = lightComponent.Id,
                    MovingData = BoardSerializer.SerializeToString(lightComponent.SetterBoard),
                    Title = lightComponent.Identifier,
                };
            }

            return null;
        }

        private void CopyStream(Stream src, Stream dest)
        {
            var buffer = new byte[src.Length];
            int len;
            while ((len = src.Read(buffer, 0, buffer.Length)) > 0)
            {
                dest.Write(buffer, 0, len);
            }
        }

    }
}
