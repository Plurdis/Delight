using Delight.Components.Medias;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Components.Common
{
    /// <summary>
    /// 사용자가 추가한 미디어를 관리합니다.
    /// </summary>
    public class MediaManager
    {
        public event PropertyChangedEventHandler MediaChanged;

        private Dictionary<string, Media> _medias;

        public MediaManager()
        {
            _medias = new Dictionary<string, Media>();
        }
        
        public Media GetMediaFromPath(string filePath)
        {
            MediaTypes type = MediaTools.GetMediaTypeFromFile(filePath);
            FileInfo fi = new FileInfo(filePath);
            switch (type)
            {
                case MediaTypes.Image:
                    return new Image()
                    {
                        Identifier = fi.Name,
                        OriginalPath = fi.FullName,
                        Time = TimeSpan.FromSeconds(10),
                    };
                case MediaTypes.Sound:
                    return new Sound()
                    {
                        Identifier = fi.Name,
                        OriginalPath = fi.FullName,
                        Time = MediaTools.GetMediaDuration(filePath),
                    };
                case MediaTypes.Video:
                    return new Video()
                    {
                        Identifier = fi.Name,
                        OriginalPath = fi.FullName,
                        Time = MediaTools.GetMediaDuration(filePath),
                    };
                default:
                    return null;
            }
        }

        public void RemoveMedia(Media media)
        {
            var item = _medias.First(kvp => kvp.Value == media);

            _medias.Remove(item.Key);
        }

        public void AddImage(Image image)
        {
            AddMedia(image);
        }

        public void AddSound(Sound sound)
        {
            AddMedia(sound);
        }

        public void AddVideo(Video video)
        {
            AddMedia(video);
        }

        private void AddMedia(Media media)
        {
            MediaChanged?.Invoke(this, new PropertyChangedEventArgs("MediaManager"));
            _medias[media.Identifier] = media;
        }
    }
}
