using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Media
{
    /// <summary>
    /// 사용자가 추가한 미디어를 관리합니다.
    /// </summary>
    public class MediaManager
    {
        public MediaManager()
        {
            _medias = new Dictionary<string, IMedia>();
        }
        
        public Image AddImageFromPath(string filePath)
        {
            return null;
        }

        public Sound AddSoundFromPath(string filePath)
        {
            return null;
        }

        public Video AddVideoFromPath(string filePath)
        {
            return null;
        }

        private Dictionary<string, IMedia> _medias;

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

        private void AddMedia(IMedia media)
        {
            _medias[media.Identity] = media;
        }
    }
}
