using Delight.Core.Template.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace Delight.Core.Template
{
    public class YoutubeDownloader
    {
        public YoutubeDownloader()
        {
            string link = "https://www.youtube.com/watch?v=DCcL4VZyups&ab_channel=%EC%9C%A0%EC%A4%80%ED%98%B8";

            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);

            foreach (VideoInfo vi in videoInfos)
            {
                
                Console.WriteLine(vi.Resolution + "-" + vi.VideoExtension);
            }

            VideoInfo video = videoInfos.First();

            video.

            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }


            var videoDownloader = new VideoDownloader(video, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.mp4");

            //videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);

            videoDownloader.Execute();
        }

        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        private static Regex regexExtractId = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
        private static string[] validAuthorities = { "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };

        public string ExtractVideoIdFromUri(Uri uri)
        {
            try
            {
                string authority = new UriBuilder(uri).Uri.Authority.ToLower();

                //check if the url is a youtube url
                if (validAuthorities.Contains(authority))
                {
                    //and extract the id
                    var regRes = regexExtractId.Match(uri.ToString());
                    if (regRes.Success)
                    {
                        return regRes.Groups[1].Value;
                    }
                }
            }
            catch { }


            return null;
        }

        public YoutubeSource GetYoutubeSource(string link)
        {
            Uri uri = new Uri(link);

            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);

            var info = videoInfos.First();

            var youtubeSource = new YoutubeSource();

            var id = ExtractVideoIdFromUri(uri);

            youtubeSource.ThumbnailUri = $@"{"https"}://img.youtube.com/vi/{id}/0.jpg";
            youtubeSource.Title = info.Title;
            youtubeSource.DownloadUrl = 
        }
    }
}
