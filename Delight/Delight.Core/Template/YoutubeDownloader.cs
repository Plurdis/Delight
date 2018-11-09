using Delight.Core.Extensions;
using Delight.Core.Sources;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YoutubeExplode;
using YoutubeRequest = Google.Apis.YouTube.v3.VideosResource.ListRequest;

namespace Delight.Core.Template
{
    public static class YoutubeDownloader
    {
        public static string ApiKey = "AIzaSyBe_BEbcHZEVrXs-_ciKV3OoRoovktNrf0";

        static YoutubeDownloader()
        {
            //string link = "https://www.youtube.com/watch?v=DCcL4VZyups&ab_channel=%EC%9C%A0%EC%A4%80%ED%98%B8";

            //IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);

            //foreach (VideoInfo vi in videoInfos)
            //{
                
            //    Console.WriteLine(vi.Resolution + "-" + vi.VideoExtension);
            //}

            //VideoInfo video = videoInfos.First();

            //video.

            //if (video.RequiresDecryption)
            //{
            //    DownloadUrlResolver.DecryptDownloadUrl(video);
            //}


            //var videoDownloader = new VideoDownloader(video, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.mp4");

            ////videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);

            //videoDownloader.Execute();
        }

        private static (string, string) GetYoutubeInfo(string id)
        {
            YoutubeRequest request = BuildRequest(id);
            VideoListResponse response = request.Execute();

            if (response.Items.Count == 0)
            {
                return (string.Empty, string.Empty);
            }

            VideoSnippet snippet = response.Items[0].Snippet;

            return (snippet.Title, GetThumbnailUrl(snippet.Thumbnails));
        }

        public static string GetThumbnailUrl(ThumbnailDetails thumbnailDetails)
        {
            if (thumbnailDetails.Standard != null)
                return thumbnailDetails.Standard.Url;
            else if (thumbnailDetails.Medium != null)
                return thumbnailDetails.Medium.Url;
            else if (thumbnailDetails.High != null)
                return thumbnailDetails.High.Url;

            return null;
        }

        public static YoutubeRequest BuildRequest(string query)
        {
            YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ApiKey
            });

            YoutubeRequest listRequest = youtube.Videos.List("snippet");
            listRequest.Id = query;

            return listRequest;
        }

        /// <summary>
        /// 공식 API를 사용한 Youtube Source 가져오기
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static YoutubeSource GetYoutubeSource_Offical(string link)
        {
            string id = link.ParseVideoId();

            (string, string) infos = GetYoutubeInfo(id);

            string title = infos.Item1;
            string thumbnailUrl = infos.Item2;

            var youtubeSource = new YoutubeSource
            {
                ThumbnailUri = thumbnailUrl,
                Title = title,
                Id = id,
            };

            return youtubeSource;
        }

        public static YoutubeSource GetYoutubeSource(string link)
        {
            string id = link.ParseVideoId();

            var client = new YoutubeClient();

            YoutubeExplode.Models.Video video = client.GetVideoAsync(id).Result;

            var youtubeSource = new YoutubeSource
            {
                ThumbnailUri = video.Thumbnails.MediumResUrl,
                Title = video.Title,
                Id = id
            };
            return youtubeSource;
        }
    }
}
