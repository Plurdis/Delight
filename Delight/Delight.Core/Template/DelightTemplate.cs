using Delight.Component.Common;
using Delight.Component.MovingLight.Effects;
using Delight.Core.Sources;
using Delight.Core.Stage.Components;
using Delight.Core.Stage.Components.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
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
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception)
                {
                    MessageBox.Show("패키징 하려는 아이템의 경로가 이미 사용중입니다.");
                    return;
                }
            }

            using (FileStream zipToOpen = new FileStream(filePath, FileMode.CreateNew))
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

                    if (DeployingPositions != null)
                    {
                        entry = archive.CreateEntry("DeployingPosition");
                        using (StreamWriter sw = new StreamWriter(entry.Open()))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(sw, DeployingPositions);
                        }
                    }
                    
                }
            }

            MessageBox.Show("아이템 패킹이 완료되었습니다.");
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string DelightAppPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Delight");

        public static string TempDelightTemplate => Path.Combine(DelightAppPath, $"Template Extracted_{RandomString(5)}");

        public static DelightTemplate FromFile(string path)
        {
            var template = new DelightTemplate();
            var baseSources = new List<BaseSource>();

            string tempPath = TempDelightTemplate;

            ZipFile.ExtractToDirectory(path, tempPath);

            FileInfo[] files = new DirectoryInfo(tempPath).GetFiles();

            foreach (FileInfo itm in files)
            {
                if (itm.Name == "DeployingPosition")
                {
                    StreamReader sr = new StreamReader(File.Open(itm.FullName, FileMode.Open));
                    string data = sr.ReadToEnd();
                    JsonSerializer serializer = new JsonSerializer();
                    var positions = (List<ItemPosition>)serializer.Deserialize(new StringReader(data), typeof(List<ItemPosition>));

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
