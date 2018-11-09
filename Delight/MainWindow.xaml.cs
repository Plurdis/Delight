using Delight.Component.Controls;
using Delight.Component.ItemProperties;
using Delight.Component.Primitives.Controllers;
using Delight.Core.Common;
using Delight.Core.MovingLight;
using Delight.Core.MovingLight.Effects;
using Delight.Core.Stage;
using Delight.Core.Stage.Components;
using Delight.Core.Template;
using Delight.Core.Template.Items;
using Delight.Core.Template.Options;
using Delight.ViewModel;
using Delight.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace Delight
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel mwViewModel;
        PlayWindow pw;

        bool dragStart;
        ListBox dragSource;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMenuFiles();

            lbMediaItem.PreviewMouseLeftButtonDown += lbMediaItem_PreviewMouseLeftButtonDown;
            lbMediaItem.PreviewMouseLeftButtonUp += lbMediaItem_PreviewMouseLeftButtonUp;
            lbMediaItem.PreviewMouseMove += lbMediaItem_PreviewMouseMove;

            ItemHeader.SelectionChanged += (s, e) =>
            {
                if (ItemHeader.SelectedItem is ListBoxItem selItem)
                {
                    int itm = int.Parse(selItem.Tag.ToString());
                    mwViewModel.FilterType = (SourceType)itm;
                }
            };

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;

            tl.ItemSelected += Tl_ItemSelected;

            propGrid.SelectedObject = new VideoItemProperty();
        }

        private void Tl_ItemSelected(object sender, EventArgs e)
        {
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            pw = new PlayWindow();
            pw.Show();
            pw.ConnectTimeLine(tl);

            tl.FrameRate = FrameRate._60PFS;

            tl.AddTrack(SourceType.Video, 1);
            tl.AddTrack(SourceType.Video, 2);
            tl.AddTrack(SourceType.Video, 3);
            tl.AddTrack(SourceType.Light, 1);
            tl.AddTrack(SourceType.Light, 2);
            tl.AddTrack(SourceType.Light, 3);
            tl.AddTrack(SourceType.Light, 4);
            tl.AddTrack(SourceType.Image, 1);

            // DEBUG: 디버깅 코드
            LightBoard board = new LightBoard();

            board.Identifier = "좌우로 움직이기";



            board.AddState(new LightState(0, 28, 254, 0, 33, 0, 0, 0, 0, 0, 0, 0));
            board.AddDelayState(new DelayState(2500, 0, 85, 254, 0, 33, 0, 0, 0, 0, 0, 0, 0));
            board.AddWait(200);
            board.AddDelayState(new DelayState(2500, 0, 28, 254, 0, 33, 0, 0, 0, 0, 0, 0, 0));
            board.AddWait(200);

            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "LightBoard.xml");
            EffectSerializer.SaveSourcesFromList(board, path);
            
            GlobalViewModel.MainWindowViewModel.MediaItems.Add(
                new LightComponent(EffectSerializer.GetStatesFromFile(path)));
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                PlayExecuted(this, null);
                e.Handled = true;
            }

        }
        public void PlayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (tl.IsReady)
                return;

            if (!tl.IsRunning)
                tl.Play();
            else
                tl.Stop();
        }


        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void InitializeMenuFiles()
        {
            mwViewModel = GlobalViewModel.MainWindowViewModel;

            this.DataContext = mwViewModel;

            //topMenu.ItemsSource = mwViewModel.Menus;
            //lbMediaItem.ItemsSource = mwViewModel.MediaItemsView;

            CommandBindings.Add(new CommandBinding(mwViewModel.OpenFileCommand, mwViewModel.OpenFileExecuted));
            CommandBindings.Add(new CommandBinding(mwViewModel.TemplateShopCommand, mwViewModel.TemplateShopExecuted));
            CommandBindings.Add(new CommandBinding(mwViewModel.EditTimeLineCommand, mwViewModel.EditTimeLineExecuted));
            CommandBindings.Add(new CommandBinding(mwViewModel.GetExternalSourceCommand, mwViewModel.GetExternalSourceExecuted));

        }

        private void lbMediaItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = true;
            ListBox parent = (ListBox)sender;
            var data = GetDataFromListBox(parent, e.GetPosition(parent));
            if (data == null)
                parent.SelectedIndex = -1;
        }

        private void lbMediaItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStart = false;
        }

        private void lbMediaItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart)
            {
                dragStart = false;
                ListBox parent = (ListBox)sender;
                dragSource = parent;
                var data = GetDataFromListBox(dragSource, e.GetPosition(parent));

                if (data is StageComponent item)
                {
                    DragDrop.DoDragDrop(parent, item, DragDropEffects.Move);
                }
            }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DMXController.Reset();
        }

        //override 
    }
}
