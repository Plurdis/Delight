using Delight.Component.Controls;
using Delight.Component.ItemProperties;
using Delight.Core.Common;
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
        TemplateShopViewModel svViewModel;

        PlayWindow pw;

        bool dragStart;
        ListBox dragSource;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMenuFiles();

            svViewModel = new TemplateShopViewModel();

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().Value)
            {
                svViewModel.LoadTemplates(ofd.FileName);
            }

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
            try
            {
                TemplateManager.SaveSourcesFromList(new ExternalSources(new BaseSource[]
                {
                    new YoutubeSource("Name1", null),
                    new YoutubeSource("Name2", null),
                }), Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            /* <ComboBoxItem Content="화질 높음 (추천)" />
                            <ComboBoxItem Content="화질 보통" />
                            <ComboBoxItem Content="화질 낮음" />*/
            YoutubeSource ys = new YoutubeSource()
            {
                Options = new List<BaseOption>(new YoutubeOption[] 
                {
                    new YoutubeOption()
                    {
                        Name = "화질 높음 (추천)"
                    },
                    new YoutubeOption()
                    {
                        Name = "화질 보통"
                    }
                }),
                Title = "타이ㅣㅣㅣㅣㅣㅣㅣ틀",
            };

            templateShop.DataContext = ys;
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

            tl.AddTrack(SourceType.Video);
            tl.AddTrack(SourceType.Video);
            tl.AddTrack(SourceType.Video);
            tl.AddTrack(SourceType.Light);
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
            mwViewModel = new MainWindowViewModel();

            this.DataContext = mwViewModel;

            //topMenu.ItemsSource = mwViewModel.Menus;
            //lbMediaItem.ItemsSource = mwViewModel.MediaItemsView;

            CommandBindings.Add(
                new CommandBinding(mwViewModel.OpenFileCommand, mwViewModel.OpenFileExecuted));
            CommandBindings.Add(
                new CommandBinding(mwViewModel.TemplateShopCommand, mwViewModel.TemplateShopExecuted));
            CommandBindings.Add(
                new CommandBinding(mwViewModel.EditTimeLineCommand, mwViewModel.EditTimeLineExecuted));
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

        //override 
    }
}
