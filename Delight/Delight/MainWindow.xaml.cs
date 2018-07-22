using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

using Delight.Common;
using Delight.Projects;
using Delight.Windows;

using LocalCommandManager = Delight.Common.InputGestureManager;

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
            MouseManager.Init();

            InfoWindow window = new InfoWindow();
            window.ShowDialog();

            this.Closing += (s, e) => Environment.Exit(0);
            ((INotifyCollectionChanged)lbItem.Items).CollectionChanged += lbItem_CollectionChanged;
            rbBox.Checked += RbBox_Checked;
            rbList.Checked += RbList_Checked;

#if DEBUG
            //img.Source = ImageCreator.GetWireFrame(200, 300, Brushes.Red);
#endif
            CommandBindings.Add(new CommandBinding(MenuCommands.ExitCommand, ExitCommandExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenFileCommand, OpenFileExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.ExportCommand, ExportExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.NewProjectCommand, NewProjectExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenProjectCommand, OpenProjectExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveAsCommand, SaveAsExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveCommand, SaveExecuted));

            SetProject(new ProjectInfo()
            {
                ProjectName = "EmptyProject1"
            });

            mediaPlayer.Open(new Uri(@"D:\Program Files\League Of Legends\Riot Games\League of Legends\RADS\projects\lol_air_client\releases\0.0.1.13\deploy\mod\lgn\themes\loginCamille\flv\login-loop.flv", UriKind.Absolute));

            //mediaPlayer.Open()
            mediaPlayer.MediaEnded += (s, e) => { mediaPlayer.Close(); };
            //FFMpegConverter converter = new FFMpegConverter();

            //MediaTools.GetMediaFile(out string path);
            //MediaTools.GetMediaFile(out string path2);
            //converter.ConcatMedia(new string[] { path, path2 }, "sample.mp4", Format.mp4, new ConcatSettings()
            //{
                
            //});
            
            //var timer = new TimeLineTimer(tl.FrameRate);

            //int i = 0;

            //timer.Tick += () => 
            //{
            //    Dispatcher.Invoke(() =>
            //    {
            //        tl.Value = i++;
            //    });
                
            //};

            //timer.Start();


            //var converter = new FFMpegConverter();

            // Get Thumbnail

            //converter.GetVideoThumbnail(@"C:\Users\uutak\Downloads\Video\small.mp4", @"C:\Users\uutak\Downloads\Video\test.jpeg");

            //
            //var test = new FFMpegConverter();

            //converter.ConvertMedia(basePath + "small.mp4",null, basePath + "test.flv", Format.flv, new ConvertSettings()
            //{
            //    // FFMPEG를 CMD로 사용하는 방법에 대해 연구해보기
            //});

            //converter.ConvertProgress += Converter_ConvertProgress;

        }

        private void RbList_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void RbBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void lbItem_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            noItemInfo.Visibility = (lbItem.Items.Count == 0) ? Visibility.Visible : Visibility.Hidden;
        }

        #region [  Global Variable  ]

        ProjectInfo ProjectInfo { get; set; }

        #endregion

        public void SetProject(ProjectInfo projectInfo)
        {
            ProjectInfo = projectInfo;
            projectInfo.PropertyChanged += ProjectInfo_PropertyChanged;

            this.Title = "Delight - " + projectInfo.ProjectName;
        }

        private void ProjectInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ProjectInfo pi)
            {
                switch (e.PropertyName)
                {
                    case "ProjectName":
                        this.Title = "Delight - " + pi.ProjectName;
                        break;
                }
            }
        }

        private void Menu_MouseMove(object sender, MouseEventArgs e)
        {
            string id = ((UIElement)sender).Uid;

            this.Title = id;
        }
    }
}
