using Delight.Core.Common;
using Delight.Core.MovingLight;
using Delight.Core.MovingLight.Effects;
using Delight.Core.MovingLight.Effects__;
using Delight.Core.Stage;
using Delight.Core.Stage.Components;
using Delight.ViewModel;
using Delight.Windows;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            tl.ItemDeselected += Tl_ItemDeselected;
            //propGrid.SelectedObject = new VideoItemProperty();

            SetterBoard sb = new SetterBoard();


            sb.AddSetterProperties(PortNumber.Brightness, "깜빡임 정도");
            sb.AddSetterProperties(PortNumber.Color, "색깔");
            sb.AddSetterGroup();
            sb.AddSetterGroup();

            sb[0].AddStaticState(1, 220);
            sb[0].AddWait(1000);
            sb[0].AddPropertyState(1, "Color");

            sb[1].AddStaticState(2, 40);
            sb[1].AddStaticState(5, 100);
            sb[1].AddStaticState(4, 200);
            sb[1].AddContinueLine(1000);
            sb[1].AddStaticState(4, 253);

            string path = @"C:\Users\uutak\바탕 화면\GroupTest.xml";

            BoardSerializer.Save(sb, path);

            var board = BoardSerializer.Load(path);
            Console.WriteLine("Done");

            LightBoard lb = new LightBoard();


            LightBoard lightBoard = new LightBoard();

            lightBoard.AddState(new LightState(45, 100, 254, 0, 42, 0, 0, 0, 0, 0, 0, 0));
            lightBoard.AddDelayState(new DelayState(1000, 45, 162, 254, 0, 42, 0, 0, 0, 0, 0, 0, 0));
            lightBoard.AddWait(200);
            lightBoard.AddDelayState(new DelayState(1000, 45, 100, 254, 0, 42, 0, 0, 0, 0, 0, 0, 0));
            lightBoard.AddWait(200);

            lightBoard.Identifier = "손 흔들기";

            var lightComponent2 = new LightComponent(lightBoard);

            GlobalViewModel.MainWindowViewModel.MediaItems.Add(lightComponent2);

            var lightEffect = EffectSerializer.GetStatesFromFile(@"C:\Users\uutak\바탕 화면\LightBoard.xml");

            var lightComponent = new LightComponent(lightEffect);

            LightBoard lightBoard2 = new LightBoard();

            lightBoard2.AddState(new LightState(173, 40, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));
            lightBoard2.AddWait(100);
            lightBoard2.AddDelayState(new DelayState(200, 188, 80, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));
            lightBoard2.AddWait(100);
            lightBoard2.AddDelayState(new DelayState(200, 173, 40, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));
            lightBoard2.AddWait(100);
            lightBoard2.AddDelayState(new DelayState(200, 158, 80, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));
            lightBoard2.AddWait(100);
            lightBoard2.AddDelayState(new DelayState(200, 173, 40, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));

            lightBoard2.Identifier = "Swing!";

            GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(lightBoard2));

            GlobalViewModel.MainWindowViewModel.MediaItems.Add(lightComponent);
            //Console.WriteLine("CRC-32(File) is {0}", Crc32.GetHashFromFile(@"C:\Users\uutak\바탕 화면\LightBoard.xml"));

            //dynamic _employee = new DynamicProperty();
            //_employee.asdf = "John";
            //_employee.LastName = "Doe";
            //_employee.Test = "This is a test";

            //MessageBox.Show(PropertyManager.GetProperty(_employee, "Test"));
            //propGrid.SelectedObject = _employee;
        }

        private void Tl_ItemDeselected(object sender, EventArgs e)
        {
            propGrid.SelectedObject = null;
        }

        private void Tl_ItemSelected(object sender, EventArgs e)
        {
            propGrid.SelectedObject = tl.SelectedItem.Property;
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
