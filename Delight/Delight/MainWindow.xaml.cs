using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
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
using Delight.Controls;
using Delight.Core.Common;
using Delight.Projects;
using Delight.Windows;

using NReco.VideoConverter;
using LocalCommandManager = Delight.Common.CommandManager;


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
            ((INotifyCollectionChanged)lbItem.Items).CollectionChanged += lbItem_CollectionChanged;

#if DEBUG
            //img.Source = ImageCreator.GetWireFrame(200, 300, Brushes.Red);
#endif
            CommandBindings.Add(new CommandBinding(MenuCommands.ExitCommand, (s, e) => Environment.Exit(0)));
            CommandBindings.Add(new CommandBinding(MenuCommands.ExportCommand, (s, e) => MessageBox.Show("[내보내기]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.NewProjectCommand, (s, e) => MessageBox.Show("[새로운 프로젝트]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenFileCommand, (s, e) =>
            {
                if (MediaTools.GetMediaFile(out string location))
                {
                    lbItem.Items.Add(new TemplateItem() { Content = new FileInfo(location).Name, Description = "File" });
                }
            }));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenProjectCommand, (s, e) => MessageBox.Show("[프로젝트 열기]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveAsCommand, (s, e) => MessageBox.Show("[다른 이름으로 저장]은 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveCommand, (s, e) => MessageBox.Show("[저장]은 완성되지 않은 기능입니다.")));
            
            MenuCommands.ExitCommand.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));

            SetProject(new ProjectInfo()
            {
                ProjectName = "EmptyProject1"
            });

            mediaPlayer.Open(new Uri(@"D:\Program Files\League Of Legends\Riot Games\League of Legends\RADS\projects\lol_air_client\releases\0.0.1.13\deploy\mod\lgn\themes\loginCamille\flv\login-loop.flv", UriKind.Absolute));
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

        private void lbItem_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            noItemInfo.Visibility = (lbItem.Items.Count == 0) ? Visibility.Visible : Visibility.Hidden;
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
