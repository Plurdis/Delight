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
        #region [  Static Members  ]

        public Image GetImageFromPath(string filePath)
        {
            return null;
        }

        public Sound GetSoundFromPath(string filePath)
        {
            return null;
        }

        public Video GetVideoFromPath(string filePath)
        {
            return null;
        }

        #endregion

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
