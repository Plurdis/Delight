using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Delight.Commands;
using Delight.Common;
using Delight.Components;
using Delight.Components.Common;
using Delight.Components.Medias;
using Delight.Controls;
using Delight.LogManage;
using Delight.Projects;
using Delight.Timing;
using Delight.Timing.Controller;
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
            #region [  Initalize  ]

            LocalCommandManager.Init();
            InitializeComponent();

            #endregion

            //RGB2HSL(Color.FromRgb(5, 215, 5), out double hh, out double ss, out double ll);

            //sliderHueMin.Value = hh;
            //sliderHueMax.Value = hh;

            //sliderSatMax.Value = ss;
            //sliderSatMin.Value = ss;

            //sliderLightMax.Value = ll;
            //sliderLightMin.Value = ll;

            //sliderSmooth.Value = 0.288;

            //MessageBox.Show($"{hh}, {ss}, {ll}");
            
            #region [  EventHandler Connect  ]

            ((INotifyCollectionChanged)lbItem.Items).CollectionChanged += lbItem_CollectionChanged;

            rbBox.Checked += RbBox_Checked;
            rbList.Checked += RbList_Checked;

            lbItem.PreviewMouseLeftButtonDown += LbItem_PreviewMouseLeftButtonDown;
            lbItem.PreviewMouseMove += LbItem_PreviewMouseMove;
            lbItem.PreviewMouseLeftButtonUp += LbItem_PreviewMouseLeftButtonUp;

            LogManager.InfoTextChanged += LogManager_InfoTextChanged;
            LogManager.InfoProgressChanged += LogManager_InfoProgressChanged;
            LogManager.InfoMaximumChanged += LogManager_InfoMaximumChanged;
            
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.Closed += (s, e) => Environment.Exit(0);
            this.Loaded += MainWindow_Loaded;

            #endregion

            #region [  CommandBindings Adding  ]

            CommandBindings.Add(new CommandBinding(MenuCommands.ExitCommand, ExitCommandExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenFileCommand, OpenFileExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.ExportCommand, ExportExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.NewProjectCommand, NewProjectExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenProjectCommand, OpenProjectExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveAsCommand, SaveAsExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveCommand, SaveExecuted));

            CommandBindings.Add(new CommandBinding(MenuCommands.ViewInfoCommand, ViewInfoExecuted));

            CommandBindings.Add(new CommandBinding(MenuCommands.ManageTemplateCommand, ManageTemplateExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.ManageDeviceCommand, ManageDeviceExecuted));

            CommandBindings.Add(new CommandBinding(ControlCommands.PlayCommand, PlayExecuted));

            CommandBindings.Add(new CommandBinding(TrackItemCommands.DeleteCommand, DeleteCommandExecuted));


            //#if DEBUG
            CommandBindings.Add(new CommandBinding(DebugCommands.PlayWindowVisibleCommand, PlayWindowVisibleExecuted));
            CommandBindings.Add(new CommandBinding(DebugCommands.UnityPreviewVisibleCommand, UnityPreviewVisibleCommandExecuted));
            CommandBindings.Add(new CommandBinding(DebugCommands.CallCustomDebugMethodCommand, CallCustomDebugMethodCommandExecuted));
            //#endif
            
            #endregion

            SetProject(new ProjectInfo()
            {
                ProjectName = "EmptyProject1"
            });

            //LoadUnityDebug();

            //AddItem(@"C:\Program Files\WindowsApps\Microsoft.Windows.Photos_2018.18051.18420.0_x64__8wekyb3d8bbwe\AppCS\Assets\WelcomePage\620x252_MakeMovies.mp4");

            #region [  조명 X축 아이템  ]

            var image = new BitmapImage(new Uri("pack://application:,,,/Delight;component/Resources/defaultLightimage.png", UriKind.Absolute));

            lbItem.Items.Add(new TemplateItem()
            {
                //Content = fi.Name,
                //ItemName = "Local Image File",
                ItemName = "조명 X축",
                Source = image,
                StageComponent = new MovingLight()
                {
                    Identifier = "조명 X축",
                    Time = TimeSpan.FromSeconds(20),
                    Thumbnail = image,
                    MovingPreset = "moveToX",
                },
            });

            lbItem.Items.Add(new TemplateItem()
            {
                //Content = fi.Name,
                //ItemName = "Local Image File",
                ItemName = "조명 Y축",
                Source = image,
                StageComponent = new MovingLight()
                {
                    Identifier = "조명 Y축",
                    Time = TimeSpan.FromSeconds(20),
                    Thumbnail = image,
                    MovingPreset = "moveToY",
                },
            });

            #endregion

            anEditor.SetTimeLine(tl);

            tl.FrameRate = Core.Common.FrameRate._60FPS;
            tl.FrameChanged += (s, e) =>
            {
                tbTime.Text = MediaTools.GetTimeText(tl.Position, tl.FrameRate);
            };

            tl.FrameMouseChanged += (s, e) =>
            {
                tl.Stop();
            };

            tl.ItemSelected += (s, e) =>
            {
                tiEditor.SetTrackItem((TrackItem)s);
                anEditor.SetTrackItem((TrackItem)s);
            };

            tl.ItemDeselected += (s, e) =>
            {
                tiEditor.SetTrackItem(null);
                anEditor.SetTrackItem(null);
            };
            
            //tbSelectItem.Inlines.Add(new Run("아이템s!")
            //{
            //    FontWeight = FontWeights.Bold,
            //    Foreground = Brushes.Red,
            //});
        }

        private void DeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TrackItem item = tl.SelectedItem;

            Track track = ((FrameworkElement)item.Parent).TemplatedParent as Track;

            track.RemoveItem(item);
            //MessageBox.Show(sender.ToString());
        }

        public void LoadUnityDebug()
        {
            //MediaTools.GetFile("사진|*.png", out string floc);

            //ImageSource thumbnail = new BitmapImage(new Uri(floc));
            ////#if DEBUG
            //if (MediaTools.GetFile("시각화 실행 파일(*.exe)|*.exe", out string fileLoc))
            //{
            //    UnityContainerLoader loader = new UnityContainerLoader(fileLoc, this, unityPanel);

            //    lbItem.Items.Add(new TemplateItem()
            //    {
            //        ItemName = "Stage Visualize Component",
            //        Source = thumbnail,
            //        StageComponent = new Unity()
            //        {
            //            Time = TimeSpan.FromMinutes(3),
            //            Identifier = "Stage Visualize Component",
            //            Thumbnail = thumbnail,
            //        },
            //    });
            //}
            //else
            //{
            //    unityBg.Visibility = Visibility.Hidden;
            //}
            //#endif
        }

        #region [  Global Veriable  ]
        
        PlayWindow pw;

        bool dragStart;
        ListBox dragSource;

        #endregion

        #region [  Global Property  ]

        ProjectInfo ProjectInfo { get; set; }

        #endregion



        #region [  LogManager Events  ]

        private void LogManager_InfoTextChanged(string text)
        {
            Dispatcher.Invoke(() =>
            {
                tbInfo.Text = text;
            });
        }

        private void LogManager_InfoProgressChanged(double value)
        {
            Dispatcher.Invoke(() =>
            {
                pbInfo.Value = value;
            });
        }

        private void LogManager_InfoMaximumChanged(double value)
        {
            Dispatcher.Invoke(() =>
            {
                pbInfo.Maximum = value;
            });
        }

        #endregion

        #region [  MainWindow's Events  ]

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bg.Height = 1080 / (1920 / bg.ActualWidth);
            bg.SizeChanged += (s, ev) =>
            {
                bg.Height = 1080 / (1920 / bg.ActualWidth);
            };
            pw = new PlayWindow();
            pw.ConnectTimeLine(tl);
            pw.Loaded += Pw_Loaded;
            pw.Show();
            
        }

        private void Pw_Loaded(object sender, RoutedEventArgs e)
        {
            tl.AddTrack(TrackType.Image);
            tl.AddTrack(TrackType.Video);
            tl.AddTrack(TrackType.Video);
            tl.AddTrack(TrackType.Effect, 1);
            tl.AddTrack(TrackType.Effect, 2);
            tl.AddTrack(TrackType.Sound);
            tl.AddTrack(TrackType.Light);
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                PlayExecuted(this, null);
                e.Handled = true;
            }
        }

        private void LbItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = true;
            ListBox parent = (ListBox)sender;
            ListBoxItem data = (ListBoxItem)GetDataFromListBox(parent, e.GetPosition(parent));
            if (data == null)
                parent.SelectedIndex = -1;
        }

        private void LbItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStart = false;   
        }

        private void LbItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart)
            {
                dragStart = false;
                ListBox parent = (ListBox)sender;
                dragSource = parent;
                ListBoxItem data = (ListBoxItem)GetDataFromListBox(dragSource, e.GetPosition(parent));

                if (data is TemplateItem item)
                {
                    DragDrop.DoDragDrop(parent, item.StageComponent, DragDropEffects.Move);
                }
            }
            
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

            //this.Title = id;
        }

        #endregion

        private static object GetDataFromListBox(ListBox source, Point point)
        {
            if (source.InputHitTest(point) is UIElement element)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                        element = VisualTreeHelper.GetParent(element) as UIElement;

                    if (element == source)
                        return null;
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }

        public void SetProject(ProjectInfo projectInfo)
        {
            ProjectInfo = projectInfo;
            projectInfo.PropertyChanged += ProjectInfo_PropertyChanged;

            this.Title = "Delight - " + projectInfo.ProjectName;
        }
    }
}
