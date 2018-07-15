using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Media
{
    /// <summary>
    /// 사용자가 추가한 미디어를 관리합니다.
    /// </summary>
    public class MediaManager
    {

        private Dictionary<string, IMedia> _medias;

        public MediaManager()
        {
            _medias = new Dictionary<string, IMedia>();
        }
        
        public IMedia AddMediaFromPath(string filePath)
        {
            return null;   
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

        private void AddMedia(IMedia media)
        {
            _medias[media.Identity] = media;
        }
    }
}
