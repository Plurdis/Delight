using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Delight.Common;
using Delight.Core.Common;
using Delight.Projects;
using Delight.Windows;

using NReco.VideoConverter;
using LocalCommandManager = Delight.Common.CommandManager;
using wf = System.Windows.Forms;

namespace Delight
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LocalCommandManager.Init();
            InitializeComponent();
            //MouseManager.Init();

            this.Closing += (s, e) => Environment.Exit(0);
#if DEBUG
            //img.Source = ImageCreator.GetWireFrame(200, 300, Brushes.Red);
#endif
            CommandBindings.Add(new CommandBinding(MenuCommands.ExitCommand, (s, e) => Environment.Exit(0)));
            CommandBindings.Add(new CommandBinding(MenuCommands.ExportCommand, (s, e) => MessageBox.Show("[내보내기]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.NewProjectCommand, (s, e) => MessageBox.Show("[새로운 프로젝트]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenFileCommand, (s, e) =>
            {

                wf.OpenFileDialog ofd = new wf.OpenFileDialog();
                // wma, aac, mp4, aiff
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
                sb.Append("*.skm;*.evo;*.nsr;*.amv;*.divx;*.webm;*.wtv;*.f4v;*.mxf;|");

                // ====================================================================================

                sb.Append("비디오 파일 (*.avi,*.wmv,*.mpg,*.mpeg,*.ts,*.3gp,*.swf,*.flv,*.mov...)|");

                sb.Append("*.avi;*.wmv;*.vmp;*.vm;*.asf;*.mpg;*.mpeg;*.mpe;*.m1v;*.m2v;" );
                sb.Append("*.mpv2;*.mp2v;*.ts;*.tp;*.tpr;*.trp;*.vob;*.ifo;*.ogm;*.ogv;" );
                sb.Append("*.mp4;*.m4v;*.m4p;*.m4b;*.3gp;*.3gpp;*.3g2;*.3gp2;*.mkv;*.rm;*.ram;" );
                sb.Append("*.rmvb;*.rpm;*.flv;*.swf;*.mov;*.qt;*.amr;*.nsv;*.dpg;*.m2ts;*.m2t;" );
                sb.Append("*.mts;*.dvr-ms;*.k3g;*.skm;*.evo;*.nsr;*.amv;*.divx;*.webm;*.wtv;*.f4v;*.mxf;|");

                // ====================================================================================

                sb.Append("오디오 파일 (*.wav,*.wma,*.mp3,*.m4a,*.aac,*.flac...)|");

                sb.Append("*.wav;*.wma;*.mpa;*.mp2;*.m1a;*.m2a;*.mp3;*.m4a;*.aac;");
                sb.Append("*.mka;*.ra;*.flac;*.ape;*.mpc;*.mod;*.ac3;*.eac3;*.dts;*.dtshd;");
                sb.Append("*.wv;*.tak;*.cda;*.dsf;*.tta;*.aiff;*.opus;");
                
                ofd.Filter = sb.ToString();
                ofd.ShowDialog();
            }));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenProjectCommand, (s, e) => MessageBox.Show("[프로젝트 열기]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveAsCommand, (s, e) => MessageBox.Show("[다른 이름으로 저장]은 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveCommand, (s, e) => MessageBox.Show("[저장]은 완성되지 않은 기능입니다.")));
            
            MenuCommands.ExitCommand.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));

            SetProject(new ProjectInfo()
            {
                ProjectName = "EmptyProject1"
            });

            mediaPlayer.Open(new Uri(@"sample", UriKind.Absolute));
            //Thread thr = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Dispatcher.Invoke(() =>
            //        {
            //            this.Title = mediaPlayer.HasVideo.ToString();
            //        });
            //        Thread.Sleep(10);
            //    }
            //});
            
            //thr.Start();

            //this.Closing += (s, e) => thr.Abort();

            var converter = new FFMpegConverter();

            // Get Thumbnail

            //converter.GetVideoThumbnail(@"C:\Users\uutak\Downloads\Video\small.mp4", @"C:\Users\uutak\Downloads\Video\test.jpeg");

            //
            var test = new FFMpegConverter();

            string basePath = @"C:\Users\uutak\Downloads\Video\";
            //converter.ConvertMedia(basePath + "small.mp4",null, basePath + "test.flv", Format.flv, new ConvertSettings()
            //{
            //    // FFMPEG를 CMD로 사용하는 방법에 대해 연구해보기
            //});

            converter.ConvertProgress += Converter_ConvertProgress;
            
            //var groups = new[]
            //{
            //    new Group { Name = ""기본 데이터" },
            //    new Group { Name = "Group2" },
            //};

            //var collectionView = new ListCollectionView(new[]
            //{
            //    new Item { Group = groups[0], Name = "Item1" },
            //    new Item { Group = groups[0], Name = "Item2" },
            //    new Item { Group = groups[1], Name = "Item3" },
            //    new Item { Group = groups[1], Name = "Item4" },
            //    new Item { Group = groups[1], Name = "Item5" },
            //    new Item { Group = groups[0], Name = "Item6" },

            //});

            //var groupDescription = new PropertyGroupDescription("Group.Name");

            //// this foreach must at least add clusters that can't be
            //// derived from items - i.e. groups with no items in them
            //foreach (var cluster in groups)
            //    groupDescription.GroupNames.Add(cluster.Name);

            //collectionView.GroupDescriptions.Add(groupDescription);
            //.ItemsSource = collectionView;
            //Clusters = groupDescription.GroupNames;

        }

        private void Converter_ConvertProgress(object sender, ConvertProgressEventArgs e)
        {
            MessageBox.Show("!");
        }

        //readonly ObservableCollection<object> Clusters;

        class Group
        {
            public string Name { get; set; }
        }

        class Item
        {
            public Group Group { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Group.Name + " :: " + Name;
            }
        }



        #region [  Global Variable  ]

        ProjectInfo ProjectInfo { get; set; }

        #endregion

        public void SetProject(ProjectInfo projectInfo)
        {
            ProjectInfo = projectInfo;
            projectInfo.PropertyChanged += ProjectInfo_PropertyChanged;

            this.Title = "SPainter - " + projectInfo.ProjectName;
        }

        private void ProjectInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ProjectInfo pi)
            {
                switch (e.PropertyName)
                {
                    case "ProjectName":
                        this.Title = "SPainter - " + pi.ProjectName;
                        break;
                }
            }
            
        }

        #region [  File I/O Task  ]

        public void OpenProject()
        {

        }

        public void SaveAs()
        {

        }

        public void Save()
        {

        }
        
        #endregion

        public void AddCommandBinding(KeyGesture gesture, ExecutedRoutedEventHandler eventHandler)
        {
            RoutedCommand comm = new RoutedCommand();

            comm.InputGestures.Add(gesture);

            this.CommandBindings.Add(new CommandBinding(comm, eventHandler));
        }

        private void Menu_MouseMove(object sender, MouseEventArgs e)
        {
            string id = ((UIElement)sender).Uid;

            this.Title = id;
        }
    }
}
