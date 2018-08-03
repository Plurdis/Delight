using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Delight.Common;
using Delight.Components;
using Delight.Components.Common;
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
            //TutorialWindow tw = new TutorialWindow();
            //tw.ShowDialog();

            MenuCommands.ExportInputGestureText = "Ctrl+Z";

            LocalCommandManager.Init();
            InitializeComponent();
            //MouseManager.Init();

            this.Closing += (s, e) => Environment.Exit(0);
            this.Loaded += MainWindow_Loaded;

            ((INotifyCollectionChanged)lbItem.Items).CollectionChanged += lbItem_CollectionChanged;
            rbBox.Checked += RbBox_Checked;
            rbList.Checked += RbList_Checked;
            lbItem.PreviewMouseLeftButtonDown += LbItem_PreviewMouseLeftButtonDown;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;

            LogManager.InfoTextChanged += LogManager_InfoTextChanged;
            LogManager.InfoProgressChanged += LogManager_InfoProgressChanged;
            LogManager.InfoMaximumChanged += LogManager_InfoMaximumChanged;

            CommandBindings.Add(new CommandBinding(MenuCommands.ExitCommand, ExitCommandExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenFileCommand, OpenFileExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.ExportCommand, ExportExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.NewProjectCommand, NewProjectExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenProjectCommand, OpenProjectExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveAsCommand, SaveAsExecuted));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveCommand, SaveExecuted));

            CommandBindings.Add(new CommandBinding(MenuCommands.ViewInfoCommand, ViewInfoExecuted));

            CommandBindings.Add(new CommandBinding(ControlCommands.PlayCommand, PlayExecuted));

            SetProject(new ProjectInfo()
            {
                ProjectName = "EmptyProject1"
            });

            reader = new TimeLineReader(tl);

            tl.FrameRate = Core.Common.FrameRate._24FPS;

            pw = new PlayWindow();
            pw.Show();
            pw.player1.PositionChanged += Player_PositionChanged;

            //pw.player1.Open()
            AddItem(@"C:\Program Files\WindowsApps\Microsoft.Windows.Photos_2018.18051.18420.0_x64__8wekyb3d8bbwe\AppCS\Assets\WelcomePage\620x252_MakeMovies.mp4");

            tl.FrameMouseChanged += async (s, e) =>
            {
                var ts = MediaTools.FrameToTimeSpan(tl.Position, tl.FrameRate);
                tl.Stop();
                allowedChange = true;
                pw.player1.Position = ts;
                allowedChange = false;

                if (pw.player1.IsPlaying)
                {
                    Thread.Sleep(10);
                    await pw.player1.Pause();
                }
            };
        }

        private void LogManager_InfoTextChanged(string text)
        {
            tbInfo.Text = text;
        }

        private void LogManager_InfoProgressChanged(double value)
        {
            pbInfo.Value = value;
        }

        private void LogManager_InfoMaximumChanged(double value)
        {
            pbInfo.Maximum = value;
        }

        int i = 0;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tl.Play();
        }

        bool allowedChange = false;

        private async void Player_PositionChanged(object sender, Unosquare.FFME.Events.PositionChangedRoutedEventArgs e)
        {
            if (allowedChange)
                return;
            if (!tl.IsRunning)
            {
                var ts = MediaTools.FrameToTimeSpan(tl.Position, tl.FrameRate);
                pw.player1.Position = ts;
                await pw.player1.Pause();

            }
        }

        PlayWindow pw;

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                PlayExecuted(this, null);
                e.Handled = true;
            }
        }

        ListBox dragSource;
        private void LbItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            dragSource = parent;
            ListBoxItem data = (ListBoxItem)GetDataFromListBox(dragSource, e.GetPosition(parent));

            if (data is TemplateItem item)
            {
                DragDrop.DoDragDrop(parent, item.StageComponent, DragDropEffects.Move);
            }
        }

        private void RbList_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbBox_Checked(object sender, RoutedEventArgs e)
        {

        }

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

        private void lbItem_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            noItemInfo.Visibility = (lbItem.Items.Count == 0) ? Visibility.Visible : Visibility.Hidden;
        }

        #region [  Global Variable  ]

        ProjectInfo ProjectInfo { get; set; }

        TimeLineReader reader { get; set; }

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
