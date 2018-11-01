using Delight.Core.Common;
using Delight.Core.Extensions;
using Delight.Core.Stage;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using NReco.VideoInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using wf = System.Windows.Forms;

namespace Delight.Component.Common
{
    public static class MediaTools
    {
        public static TimeSpan GetMediaDuration(string filePath)
        {
            if (!File.Exists(filePath))
                return TimeSpan.MinValue;

            return GetMediaInfo(filePath).Duration;
        }

        public static MediaInfo GetMediaInfo(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var probe = new FFProbe();
            return probe.GetMediaInfo(filePath);
        }

        public static BitmapImage ImageFromFile(string filePath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(filePath);
            image.DecodePixelHeight = 60;
            image.DecodePixelHeight = 60;
            image.EndInit();

            return image;
        }

        static int i = 1;

        public static ImageSource GetMediaThumbnail(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            string imagePath = Path.Combine(Path.GetTempPath(), $"temp{i++}.jpg");

            if (File.Exists(imagePath))
            {
                try
                {
                    File.Delete(imagePath);
                }
                catch (Exception)
                {
                }
            }
                

            using (var engine = new Engine())
            {
                MediaFile f1 = new MediaFile(filePath),
                    f2 = new MediaFile(imagePath);

                var opt = new ConversionOptions()
                { Seek = TimeSpan.FromSeconds(150) };

                engine.GetThumbnail(f1, f2, opt);

                int time = 100;

                while (!File.Exists(imagePath))
                {
                    opt.Seek = TimeSpan.FromSeconds(time);
                    engine.GetThumbnail(f1, f2, opt);
                    time /= 2;
                }

                //try
                //{
                //    converter.GetVideoThumbnail(filePath, imagePath, new float?(500));
                //}
                //catch (Exception)
                //{
                //    try
                //    {
                //        converter.GetVideoThumbnail(filePath, imagePath, new float?(250));
                //    }
                //    catch (Exception)
                //    {
                //        converter.GetVideoThumbnail(filePath, imagePath);
                //    }

                //}
            }
            var image = ImageFromFile(imagePath);
            //File.Delete(imagePath);
            return image;
        }

        public static string GetTimeText(int value, FrameRate frameRate)
        {
            int frameRateInt = (int)frameRate.GetEnumAttribute<DefaultValueAttribute>().Value;
            int frame = value % frameRateInt;
            int second = value / frameRateInt;
            int minute = second / 60;
            int hour = minute / 60;

            second -= minute * 60;
            minute -= hour * 60;

            return $"{hour.ToString("00")}:{minute.ToString("00")}:{second.ToString("00")};{frame}";
        }

        public static int TimeSpanToFrame(TimeSpan span, FrameRate frame)
        {
            int fr = (int)frame.GetEnumAttribute<DefaultValueAttribute>().Value;
            return ((int)Math.Truncate((span.TotalMilliseconds / 1000)) * fr) + (int)(fr * ((span.TotalMilliseconds % 1000) / 1000));
        }

        public static TimeSpan FrameToTimeSpan(int frame, FrameRate frameRate)
        {
            int fr = (int)frameRate.GetEnumAttribute<DefaultValueAttribute>().Value;
            return TimeSpan.FromSeconds((int)Math.Truncate((double)frame / fr)) + TimeSpan.FromMilliseconds((1000 / fr) * (frame % fr));
        }

        public static SourceType GetMediaTypeFromFile(string fileName)
        {
            string extension = new FileInfo(fileName).Extension;

            if (extension.StartsWith(".jpe"))
                return SourceType.Image;

            switch (extension)
            {
                case ".gif":
                case ".jpg":
                case ".png":
                case ".bmp":
                case ".dib":
                case ".tif":
                case ".wmf":
                case ".ras":
                case ".eps":
                case ".pcx":
                case ".pcd":
                case ".tga":
                    return SourceType.Image;
                case ".wav":
                case ".wma":
                case ".mpa":
                case ".mp2":
                case ".m1a":
                case ".m2a":
                case ".mp3":
                case ".m4a":
                case ".aac":
                case ".mka":
                case ".ra":
                case ".flac":
                case ".ape":
                case ".mpc":
                case ".mod":
                case ".ac3":
                case ".eac3":
                case ".dts":
                case ".dtshd":
                case ".wv":
                case ".tak":
                case ".cda":
                case ".dsf":
                case ".tta":
                case ".aiff":
                case ".opus":
                    return SourceType.Sound;
                case ".avi":
                case ".wmv":
                case ".vmp":
                case ".vm":
                case ".asf":
                case ".mpg":
                case ".mpeg":
                case ".mpe":
                case ".m1v":
                case ".m2v":
                case ".mpv2":
                case ".mp2v":
                case ".ts":
                case ".tp":
                case ".tpr":
                case ".trp":
                case ".vob":
                case ".ifo":
                case ".ogm":
                case ".ogv":
                case ".mp4":
                case ".m4v":
                case ".m4p":
                case ".m4b":
                case ".3gp":
                case ".3gpp":
                case ".3g2":
                case ".3gp2":
                case ".mkv":
                case ".rm":
                case ".ram":
                case ".rmvb":
                case ".rpm":
                case ".flv":
                case ".swf":
                case ".mov":
                case ".qt":
                case ".amr":
                case ".nsv":
                case ".dpg":
                case ".m2ts":
                case ".m2t":
                case ".mts":
                case ".dvr-ms":
                case ".k3g":
                case ".skm":
                case ".evo":
                case ".nsr":
                case ".amv":
                case ".divx":
                case ".webm":
                case ".wtv":
                case ".f4v":
                case ".mxf":
                    return SourceType.Video;
                default:
                    break;
            }
            return SourceType.Unknown;
        }

        public static bool GetProjectFile(out string fileLocation)
        {
            wf.OpenFileDialog ofd = new wf.OpenFileDialog();
            ofd.Filter = "Delight 프로젝트 파일 (*.delight)|*.delight;";

            if (ofd.ShowDialog() == wf.DialogResult.OK)
            {
                fileLocation = ofd.FileName;
                return true;
            }
            else
            {
                fileLocation = string.Empty;
                return false;
            }
        }

        public static bool GetPackageFile(out string fileLocation)
        {
            wf.OpenFileDialog ofd = new wf.OpenFileDialog();
            ofd.Filter = "Delight 패키지 파일 (*.dpack)|*.dpack;";

            if (ofd.ShowDialog() == wf.DialogResult.OK)
            {
                fileLocation = ofd.FileName;
                return true;
            }
            else
            {
                fileLocation = string.Empty;
                return false;
            }
        }

        public static bool GetFile(string filter, out string fileLocation)
        {
            wf.OpenFileDialog ofd = new wf.OpenFileDialog();
            ofd.Filter = filter;
            if (ofd.ShowDialog() == wf.DialogResult.OK)
            {
                fileLocation = ofd.FileName;
                return true;
            }
            else
            {
                fileLocation = string.Empty;
                return false;
            }
        }

        public static bool GetMediaFiles(out string[] fileLocations)
        {
            wf.OpenFileDialog ofd = new wf.OpenFileDialog();
            var sb = new StringBuilder();

            sb.Append("지원하는 모든 미디어 파일 (*.wav,*.wma,*.mp3,");
            sb.Append("*.m4a,*.aac,*.flac,*.avi,");
            sb.Append("*.wmv,*.mpg,*.mpeg,*.ts,*.3gp,*.swf,*.flv,*.mov,...)|");


            sb.Append("*.wav;*.wma;*.mpa;*.mp2;*.m1a;*.m2a;*.mp3;");
            sb.Append("*.m4a;*.aac;*.mka;*.ra;*.flac;*.ape;*.mpc;*.mod;*.ac3;*.eac3;");
            sb.Append("*.dts;*.dtshd;*.wv;*.tak;*.cda;*.dsf;*.tta;*.aiff;*.opus;*.avi;");
            sb.Append("*.wmv;*.vmp;*.vm;*.asf;*.mpg;*.mpeg;*.mpe;*.m1v;*.m2v;*.mpv2;*.mp2v;");
            sb.Append("*.ts;*.tp;*.tpr;*.trp;*.vob;*.ifo;*.ogm;*.ogv;*.mp4;*.m4v;*.m4p;*.m4b;");
            sb.Append("*.3gp;*.3gpp;*.3g2;*.3gp2;*.mkv;*.rm;*.ram;*.rmvb;*.rpm;*.flv;*.swf;");
            sb.Append("*.mov;*.qt;*.amr;*.nsv;*.dpg;*.m2ts;*.m2t;*.mts;*.dvr-ms;*.k3g;");
            sb.Append("*.skm;*.evo;*.nsr;*.amv;*.divx;*.webm;*.wtv;*.f4v;*.mxf;");
            sb.Append("*.gif;*.jpg;*.jpe*;*.png;*.bmp;*.dib;*.tif;*.wmf;*.ras;*.eps;*.pcx;*.pcd;*.tga;|");

            // ====================================================================================

            sb.Append("비디오 파일 (*.avi,*.wmv,*.mpg,*.mpeg,*.ts,*.3gp,*.swf,*.flv,*.mov...)|");

            sb.Append("*.avi;*.wmv;*.vmp;*.vm;*.asf;*.mpg;*.mpeg;*.mpe;*.m1v;*.m2v;");
            sb.Append("*.mpv2;*.mp2v;*.ts;*.tp;*.tpr;*.trp;*.vob;*.ifo;*.ogm;*.ogv;");
            sb.Append("*.mp4;*.m4v;*.m4p;*.m4b;*.3gp;*.3gpp;*.3g2;*.3gp2;*.mkv;*.rm;*.ram;");
            sb.Append("*.rmvb;*.rpm;*.flv;*.swf;*.mov;*.qt;*.amr;*.nsv;*.dpg;*.m2ts;*.m2t;");
            sb.Append("*.mts;*.dvr-ms;*.k3g;*.skm;*.evo;*.nsr;*.amv;*.divx;*.webm;*.wtv;*.f4v;*.mxf;|");

            // ====================================================================================

            sb.Append("오디오 파일 (*.wav,*.wma,*.mp3,*.m4a,*.aac,*.flac...)|");

            sb.Append("*.wav;*.wma;*.mpa;*.mp2;*.m1a;*.m2a;*.mp3;*.m4a;*.aac;");
            sb.Append("*.mka;*.ra;*.flac;*.ape;*.mpc;*.mod;*.ac3;*.eac3;*.dts;*.dtshd;");
            sb.Append("*.wv;*.tak;*.cda;*.dsf;*.tta;*.aiff;*.opus;|");

            // ====================================================================================

            sb.Append("이미지 파일 (*.gif,*.jpg,*.jpe*,*.png,*.bmp...)|");
            sb.Append("*.gif;*.jpg;*.jpe*;*.png;*.bmp;*.dib;*.tif;*.wmf;*.ras;*.eps;*.pcx;*.pcd;*.tga;");

            ofd.Filter = sb.ToString();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == wf.DialogResult.OK)
            {
                fileLocations = ofd.FileNames;
                return true;
            }
            else
            {
                fileLocations = null;
                return false;
            }
        }
    }
}
