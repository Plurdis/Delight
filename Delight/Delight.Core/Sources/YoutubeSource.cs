using Delight.Core.Sources.Options;
using Delight.Core.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace Delight.Core.Sources
{
    public class YoutubeSource : BaseSource
    {
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public event AsyncCompletedEventHandler DownloadFileCompleted;

        public override List<BaseOption> Options
        {
            get
            {
                List<BaseOption> options = new List<BaseOption>();

                options.Add(new YoutubeOption()
                {
                    Name = "다운로드 가능한 최상의 품질",
                    Tag = "1",
                });
                return options;
            }
        }

        public string Id { get; set; }

        public YoutubeSource(string title, string thumbnailUri, string link) : this()
        {
            Title = title;
            ThumbnailUri = thumbnailUri;
        }

        public YoutubeSource() : base("유튜브 영상")
        {
        }

        public override void Download(int SelectedIndex)
        {
            if (SelectedIndex == -1)
                SelectedIndex = 0;

            if (FileCacheDictionary.ContainsId(Id))
            {
                Console.WriteLine("Already Exists!");
            }
            else
            {

                YoutubeClient youtubeClient = new YoutubeClient();
                MediaStreamInfoSet streamInfoSet = youtubeClient.GetVideoMediaStreamInfosAsync(Id).Result;
                MuxedStreamInfo streamInfo = streamInfoSet.Muxed.WithHighestVideoQuality();

                if (streamInfo == null)
                    throw new Exception("다운로드 할 수 없는 영상입니다.");

                Console.WriteLine($"Audio:{streamInfo.AudioEncoding.ToString()} Quality : {streamInfo.VideoQualityLabel} Download URL : {streamInfo.Url}");

                WebClient client = new WebClient();

                client.DownloadProgressChanged += DownloadProgressChanged;
                client.DownloadFileCompleted += DownloadFileCompleted;

                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Delight\\";

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);


                string path = basePath + Path.GetRandomFileName() + ".mp4";

                client.DownloadFile(streamInfo.Url, path);
                FileCacheDictionary.AddPair(Title, path, Id);

                Console.WriteLine(path);
            }
        }
    }
}
