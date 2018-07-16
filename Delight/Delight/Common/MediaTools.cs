﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Delight.Exceptions;
using Delight.Media;

using NReco.VideoInfo;

using wf = System.Windows.Forms;

namespace Delight.Common
{
    public static class MediaTools
    {
        public static TimeSpan GetMediaDuration(string filePath)
        {
            try
            {
                var probe = new FFProbe();
                return probe.GetMediaInfo(filePath).Duration;
            }
            catch (Exception ex)
            {
                throw new ProcessException("예외 발생", ex);
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

        public static bool GetMediaFile(out string fileLocation)
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
    }
}
