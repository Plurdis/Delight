using Delight.Component.Common;
using Delight.Core.Sources;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.IO.Compression;
using System;
using Delight.Core.Stage.Components;
using Delight.Core.Stage.Components.Media;
using System.Text;

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
                var ySourceBuilder = new StringBuilder();
                foreach(BaseSource source in Sources)
                {
                    if (source is YoutubeSource ySource)
                    {
                        ySourceBuilder.Append(ySource.Id);
                    }
                    else if (source is ExternalVideoSource exVideoSource)
                    {
                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                        {
                            ZipArchiveEntry readmeEntry = archive.CreateEntry(".dlight");

                            CopyStream(File.OpenRead(@"C:\Users\장유탁\Desktop\NanumFontSetup_TTF_ALL_totalsearch.exe"), readmeEntry.Open());
                        }
                    }
                }
            }
        }

        public BaseSource ConvertToSource(StageComponent comp)
        {
            if (comp is VideoMedia media)
            {
                if (media.FromYoutube)
                {
                    return new YoutubeSource(media.Identifier, media.Thumbnail.OriginalString, media.DownloadLink);
                }
                else
                {
                    return new ExternalVideoSource()
                    {
                        FileName = media.Path,
                        ThumbnailUri = media.Thumbnail.OriginalString,
                        Title = media.Identifier,
                    };
                }
            }
            else if (comp is LightComponent lightComponent)
            {
                return new LightSource()
                {
                    SourceName = lightComponent.Identifier,
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
