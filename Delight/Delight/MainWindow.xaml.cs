using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Delight.Commands;
using Delight.Common;
using Delight.Components;
using Delight.Components.Common;
using Delight.Components.Medias;
using Delight.Controls;
using Delight.Extensions;
using Delight.LogManage;
using Delight.Projects;
using Delight.TimeLineComponents;
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

            CommandBindings.Add(new CommandBinding(ControlCommands.PlayCommand, PlayExecuted));

//#if DEBUG
            CommandBindings.Add(new CommandBinding(DebugCommands.PlayWindowVisibleCommand, PlayWindowVisibleExecuted));
            CommandBindings.Add(new CommandBinding(DebugCommands.UnityPreviewVisibleCommand, UnityPreviewVisibleCommandExecuted));
            //#endif

            #endregion

            SetProject(new ProjectInfo()
            {
                ProjectName = "EmptyProject1"
            });
            
            pw = new PlayWindow();
            pw.Show();

            LoadUnityDebug();

            AddItem(@"C:\Program Files\WindowsApps\Microsoft.Windows.Photos_2018.18051.18420.0_x64__8wekyb3d8bbwe\AppCS\Assets\WelcomePage\620x252_MakeMovies.mp4");

            tl.TimeLineReader.SetPlayer(pw.player1, pw.player2);
            tl.FrameRate = Core.Common.FrameRate._60FPS;
            tl.FrameMouseChanged += (s, e) =>
            {
                tl.Stop();
            };
            
            //tbSelectItem.Inlines.Add(new Run("아이템s!")
            //{
            //    FontWeight = FontWeights.Bold,
            //    Foreground = Brushes.Red,
            //});
        }

        public void LoadUnityDebug()
        {
            MediaTools.GetFile("사진|*.png", out string floc);

            ImageSource thumbnail = new BitmapImage(new Uri(floc));
            //#if DEBUG
            if (MediaTools.GetFile("시각화 실행 파일(*.exe)|*.exe", out string fileLoc))
            {
                UnityContainerLoader loader = new UnityContainerLoader(fileLoc, this, unityPanel);

                lbItem.Items.Add(new TemplateItem()
                {
                    ItemName = "Stage Visualize Component",
                    Source = thumbnail,
                    StageComponent = new Unity()
                    {
                        Time = TimeSpan.FromMinutes(3),
                        Identifier = "Stage Visualize Component",
                        Thumbnail = thumbnail,
                    },
                });
            }
            else
            {
                unityBg.Visibility = Visibility.Hidden;
            }
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

        int i = 0;

        #region [  MainWindow's Events  ]

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bg.Height = 1080 / (1920 / bg.ActualWidth);
            bg.SizeChanged += (s, ev) =>
            {
                bg.Height = 1080 / (1920 / bg.ActualWidth);
            };
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
