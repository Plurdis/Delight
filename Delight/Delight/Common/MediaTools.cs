using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delight.Media;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Delight.Common
{
    public static class MediaTools
    {
        public static TimeSpan GetVideoDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }

        
        public static MediaTypes GetMediaTypeFromFile(string fileName)
        {
            string extension = new FileInfo(fileName).Extension;
            switch (extension)
            {
                case ".jpg":
                case ".bmp":
                case ".png":
                case "jpeg":
                case "gif":
                    return MediaTypes.Image;
                case ".wav":
                case ".mp3":
                case ".flac":
                case ".m4a":
                    return MediaTypes.Sound;
                case ".avi":
                case "mpeg":
                case "mp4":
                case ".mov":
                case ".qt":
                    return MediaTypes.Video;
                default:
                    break;
            }
            return MediaTypes.Unknown;
        }
    }
}
