using Delight.Component.Common;
using Delight.Component.MovingLight.Effects;
using Delight.Component.MovingLight.Effects.Setters;
using Delight.Component.MovingLight.Effects.Values;
using Delight.Core.Common;
using Delight.Core.MovingLight;
using Delight.Core.Stage;
using Delight.Core.Stage.Components;
using Delight.ViewModel;
using Delight.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
            InitializeLightItems();

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
            tl.ItemRemoved += Tl_ItemRemoved;
            tl.FrameChanged += (s, e) =>
            {
                tbTime.Text = MediaTools.GetTimeText(tl.Position, tl.FrameRate);
            };

            lvEvents.Items.Clear();

            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;

            #region [  주석  ]

            //GlobalViewModel.MainWindowViewModel.AddFilesFromPath(new string[] { @"C:\Users\장유탁\AppData\Roaming\delight\ot50ya4f.yyc.mp4" });
            /*

           SetterBoard sb = new SetterBoard();
           sb.Identifier = "손 흔들기";

           sb.AddSetterProperties(PortNumber.Blink, "Blinking", "깜빡이는 정도");

           sb.AddSetterGroup();
           sb.AddSetterGroup();
           sb.SetInitalizeValue((PortNumber.XAxis, new StaticValue(45)),
               (PortNumber.YAxis, new StaticValue(100)),
               (PortNumber.Brightness, new StaticValue(254)),
               (PortNumber.Blink, new PropertyValue("Blinking")),
               (PortNumber.Color, new StaticValue(220)));
           //(PortNumber.Color, new PropertyValue("Color"))

           sb[0].AddContinueLine(1000);
           sb[0].AddStates((PortNumber.YAxis, (StaticValue)162));

           sb[0].AddWait(200);

           sb[0].AddContinueLine(1000);
           sb[0].AddStates((PortNumber.YAxis, (StaticValue)100));

           sb[0].AddWait(200);

           sb[1].AddContinueLine(500);
           sb[1].AddStates((PortNumber.XAxis, (StaticValue)50));

           sb[1].AddWait(200);

           sb[1].AddContinueLine(500);
           sb[1].AddStates((PortNumber.XAxis, (StaticValue)40));

           sb[1].AddWait(200);


           GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb));

           //(PortNumber.Blink, new PropertyValue("Blinking")


           //string path = @"C:\Users\uutak\바탕 화면\GroupTest.xml";

           //BoardSerializer.Save(sb, path);

           //var board = BoardSerializer.Load(path);
           //Console.WriteLine("Done");

           //GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(board));


           SetterBoard sb2 = new SetterBoard();

           sb2.Identifier = "좌우로 흔들기";
           sb2.AddSetterGroup();

           sb2[0].AddWait(1000);

           SetterBoard sb3 = new SetterBoard();

           sb3.Identifier = "위아래로 움직이기";
           sb3.AddSetterGroup();

           sb3[0].AddWait(1000);


           SetterBoard sb4 = new SetterBoard();

           sb4.Identifier = "깜빡이며 사선 움직임";
           sb4.AddSetterGroup();
           //sb2.AddSetterGroup();

           sb4.SetInitalizeValue((PortNumber.XAxis, new StaticValue(90)),
               (PortNumber.YAxis, new StaticValue(78)),
               (PortNumber.Brightness, new StaticValue(89)),
               (PortNumber.Blink, new StaticValue(200)),
               (PortNumber.Color, new StaticValue(220)));

           //sb2.InitalizeWaitValue

           sb4[0].AddContinueLine(400);
           sb4[0].AddStates((PortNumber.YAxis, new StaticValue(34)),
                            (PortNumber.XAxis, new StaticValue(60)));

           sb4[0].AddContinueLine(400);

           sb4[0].AddStates((PortNumber.YAxis, new StaticValue(120)),
                            (PortNumber.XAxis, new StaticValue(120)));

           SetterBoard sb5 = new SetterBoard();

           sb5.Identifier = "깜빡이며 흔들기";
           sb5.AddSetterGroup();
           sb5.SetInitalizeValue((PortNumber.XAxis, new StaticValue(90)),
               (PortNumber.YAxis, new StaticValue(78)),
               (PortNumber.Brightness, new StaticValue(89)),
               (PortNumber.Blink, new StaticValue(200)),
               (PortNumber.Color, new StaticValue(200)));

           sb5[0].AddContinueLine(300);
           sb5[0].AddStates((PortNumber.YAxis, new StaticValue(34)));

           sb5[0].AddContinueLine(300);

           sb5[0].AddStates((PortNumber.YAxis, new StaticValue(120)));


           //sb2[1].AddContinueLine(2000);

           //sb2[1].AddStates((PortNumber.XAxis, new StaticValue(95)));

           //sb2[1].AddContinueLine(2000);

           //sb2[1].AddStates((PortNumber.XAxis, new StaticValue(85)));


           //(PortNumber.Blink, new PropertyValue("Blinking")

           SetterBoard sb6 = new SetterBoard();

           sb6.Identifier = "원 그리기";
           sb6.AddSetterGroup();

           sb6[0].AddWait(1000);

           SetterBoard sb7 = new SetterBoard();

           sb7.Identifier = "8자 그리기";
           sb7.AddSetterGroup();

           sb7[0].AddWait(1000);

           SetterBoard sb8 = new SetterBoard();

           sb8.Identifier = "색 바뀌며 돌기";
           sb8.AddSetterGroup();

           sb8[0].AddWait(1000);

           GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb2));
           GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb3));
           GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb4));
           GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb5));
           GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb6));
           GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb7));
           GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb8));

             */

            //LightBoard lb = new LightBoard();


            //LightBoard lightBoard = new LightBoard();

            //lightBoard.AddState(new LightState(45, 100, 254, 0, 42, 0, 0, 0, 0, 0, 0, 0));
            //lightBoard.AddDelayState(new DelayState(1000, 45, 162, 254, 0, 42, 0, 0, 0, 0, 0, 0, 0));
            //lightBoard.AddWait(200);
            //lightBoard.AddDelayState(new DelayState(1000, 45, 100, 254, 0, 42, 0, 0, 0, 0, 0, 0, 0));
            //lightBoard.AddWait(200);

            //lightBoard.Identifier = "손 흔들기";

            //var lightComponent2 = new LightComponent(lightBoard);

            //GlobalViewModel.MainWindowViewModel.MediaItems.Add(lightComponent2);

            //var lightEffect = EffectSerializer.GetStatesFromFile(@"C:\Users\uutak\바탕 화면\LightBoard.xml");

            //var lightComponent = new LightComponent(lightEffect);

            //LightBoard lightBoard2 = new LightBoard();

            //lightBoard2.AddState(new LightState(173, 40, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));
            //lightBoard2.AddWait(100);
            //lightBoard2.AddDelayState(new DelayState(200, 188, 80, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));
            //lightBoard2.AddWait(100);
            //lightBoard2.AddDelayState(new DelayState(200, 173, 40, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));
            //lightBoard2.AddWait(100);
            //lightBoard2.AddDelayState(new DelayState(200, 158, 80, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));
            //lightBoard2.AddWait(100);
            //lightBoard2.AddDelayState(new DelayState(200, 173, 40, 254, 193, 33, 0, 0, 0, 0, 0, 0, 0));

            //lightBoard2.Identifier = "Swing!";

            //GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(lightBoard2));

            //GlobalViewModel.MainWindowViewModel.MediaItems.Add(lightComponent);
            //Console.WriteLine("CRC-32(File) is {0}", Crc32.GetHashFromFile(@"C:\Users\uutak\바탕 화면\LightBoard.xml"));

            //dynamic _employee = new DynamicProperty();
            //_employee.asdf = "John";
            //_employee.LastName = "Doe";
            //_employee.Test = "This is a test";

            //MessageBox.Show(PropertyManager.GetProperty(_employee, "Test"));
            //propGrid.SelectedObject = _employee;

            #endregion
        }

        public static string DelightAppPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Delight");

        private void InitializeLightItems()
        {
            //string lightPath = Path.Combine(DelightAppPath, "Preset Light Movement");

            //foreach (string filePath in Directory.EnumerateFiles(lightPath))
            //{
            //    SetterBoard lb = BoardSerializer.Load(filePath);

            //    AddBoard(lb);
            //}

            #region [  SetterBoard 1  ]

            SetterBoard sb1 = new SetterBoard();

            sb1.Identifier = "사선으로 움직이기 (↖)";
            sb1.AddSetterProperties(PortNumber.Color, "Color", "색깔", 1, true);
            sb1.AddSetterProperties(PortNumber.Blink, "Blinking", "깜빡이는 정도", 0, true);
            sb1.AddSetterProperties(PortNumber.Brightness, "Brightness", "밝기", 150, true);
            sb1.AddSetterProperties(PortNumber.XAxis, "StartXPosition", "시작 X 좌표 위치", 159);
            sb1.AddSetterProperties(PortNumber.YAxis, "StartYPosition", "시작 Y 좌표 위치", 50);

            sb1.AddSetterGroup();

            sb1.InitalizeValue = new List<ValueSetter>()
            {
                new ValueSetter(PortNumber.Color, "Color"),
                new ValueSetter(PortNumber.Blink, "Blinking"),
                new ValueSetter(PortNumber.Brightness, "Brightness"),
                new ValueSetter(PortNumber.XAxis, "StartXPosition"),
                new ValueSetter(PortNumber.YAxis, "StartYPosition"),
            };

            sb1[0].AddContinueLine(500);
            sb1[0].AddStates((PortNumber.XAxis, new RelativeValue("StartXPosition", 20, RelativeValue.RelativeSign.Plus)),
                (PortNumber.YAxis, new RelativeValue("StartYPosition", 60, RelativeValue.RelativeSign.Plus)));

            sb1[0].AddContinueLine(500);

            sb1[0].AddStates((PortNumber.XAxis, (PropertyValue)"StartXPosition"),
                (PortNumber.YAxis, (PropertyValue)"StartYPosition"));

            #endregion

            #region [  SetterBoard 2  ]

            SetterBoard sb2 = new SetterBoard();

            sb2.Identifier = "사선으로 움직이기 (↗)";
            sb2.AddSetterProperties(PortNumber.Color, "Color", "색깔", 1, true);
            sb2.AddSetterProperties(PortNumber.Blink, "Blinking", "깜빡이는 정도", 0, true);
            sb2.AddSetterProperties(PortNumber.Brightness, "Brightness", "밝기", 150, true);
            sb2.AddSetterProperties(PortNumber.XAxis, "StartXPosition", "시작 X 좌표 위치", 183);
            sb2.AddSetterProperties(PortNumber.YAxis, "StartYPosition", "시작 Y 좌표 위치", 50);

            sb2.AddSetterGroup();

            sb2.InitalizeValue = new List<ValueSetter>()
            {
                new ValueSetter(PortNumber.Color, "Color"),
                new ValueSetter(PortNumber.Blink, "Blinking"),
                new ValueSetter(PortNumber.Brightness, "Brightness"),
                new ValueSetter(PortNumber.XAxis, "StartXPosition"),
                new ValueSetter(PortNumber.YAxis, "StartYPosition"),
            };

            sb2[0].AddContinueLine(500);
            sb2[0].AddStates((PortNumber.XAxis, new RelativeValue("StartXPosition", 20, RelativeValue.RelativeSign.Minus)),
                (PortNumber.YAxis, new RelativeValue("StartYPosition", 60, RelativeValue.RelativeSign.Plus)));

            sb2[0].AddContinueLine(500);

            sb2[0].AddStates((PortNumber.XAxis, (PropertyValue)"StartXPosition"),
                (PortNumber.YAxis, (PropertyValue)"StartYPosition"));

            #endregion

            #region [  SetterBoard 3  ]

            SetterBoard sb3 = new SetterBoard();

            sb3.Identifier = "위 아래로 움직이기";
            sb3.AddSetterProperties(PortNumber.Color, "Color", "색깔", 1, true);
            sb3.AddSetterProperties(PortNumber.Blink, "Blinking", "깜빡이는 정도", 0, true);
            sb3.AddSetterProperties(PortNumber.Brightness, "Brightness", "밝기", 150, true);
            sb3.AddSetterProperties(PortNumber.YAxis, "StartYPosition", "시작 Y 좌표 위치", 50);

            sb3.AddSetterGroup();

            sb3.InitalizeValue = new List<ValueSetter>()
            {
                new ValueSetter(PortNumber.Color, "Color"),
                new ValueSetter(PortNumber.Blink, "Blinking"),
                new ValueSetter(PortNumber.Brightness, "Brightness"),
                new ValueSetter(PortNumber.YAxis, "StartYPosition"),
            };

            sb3[0].AddContinueLine(500);
            sb3[0].AddStates((PortNumber.YAxis, new RelativeValue("StartYPosition", 60, RelativeValue.RelativeSign.Plus)));

            sb3[0].AddContinueLine(500);

            sb3[0].AddStates((PortNumber.YAxis, (PropertyValue)"StartYPosition"));

            #endregion

            #region [  SetterBoard 4  ]

            SetterBoard sb4 = new SetterBoard();

            sb4.Identifier = "좌우로 움직이기";

            sb4.AddSetterProperties(PortNumber.Color, "Color", "색깔", 1, true);
            sb4.AddSetterProperties(PortNumber.Blink, "Blinking", "깜빡이는 정도", 0, true);
            sb4.AddSetterProperties(PortNumber.Brightness, "Brightness", "밝기", 150, true);
            sb4.AddSetterProperties(PortNumber.XAxis, "StartXPosition", "시작 X 좌표 위치", 156);
            sb4.AddSetterProperties(PortNumber.YAxis, "YPosition", "Y 좌표 위치", 50, true);

            sb4.AddSetterGroup();
            
            sb4.InitalizeValue = new List<ValueSetter>()
            {
                new ValueSetter(PortNumber.Color, "Color"),
                new ValueSetter(PortNumber.Blink, "Blinking"),
                new ValueSetter(PortNumber.Brightness, "Brightness"),
                new ValueSetter(PortNumber.XAxis, "StartXPosition"),
                new ValueSetter(PortNumber.YAxis, "YPosition"),
            };


            sb4[0].AddContinueLine(500);
            sb4[0].AddStates((PortNumber.XAxis, new RelativeValue("StartXPosition", 40, RelativeValue.RelativeSign.Plus)));
            sb4[0].AddContinueLine(500);
            sb4[0].AddStates((PortNumber.XAxis, (PropertyValue)"StartXPosition"));

            #endregion

            #region [  SetterBoard 5  ]

            var sb5 = new SetterBoard();

            sb5.Identifier = "큰 세모 그리기";

            sb5.AddSetterProperties(PortNumber.Color, "Color", "색깔", 1, true);
            sb5.AddSetterProperties(PortNumber.Blink, "Blinking", "깜빡이는 정도", 0, true);
            sb5.AddSetterProperties(PortNumber.Brightness, "Brightness", "밝기", 150, true);
            sb5.AddSetterProperties(PortNumber.XAxis, "XPosition", "시작 X 좌표 위치", 156, true);
            sb5.AddSetterProperties(PortNumber.YAxis, "YPosition", "시작 Y 좌표 위치", 30, true);

            sb5.InitalizeValue = new List<ValueSetter>()
            {
                new ValueSetter(PortNumber.Color, "Color"),
                new ValueSetter(PortNumber.Blink, "Blinking"),
                new ValueSetter(PortNumber.Brightness, "Brightness"),
                new ValueSetter(PortNumber.XAxis, "XPosition"),
                new ValueSetter(PortNumber.YAxis, "YPosition"),
            };

            sb5.AddSetterGroup();

            sb5[0].AddContinueLine(500);

            sb5[0].AddStates((PortNumber.XAxis, new RelativeValue("XPosition", 30, RelativeValue.RelativeSign.Plus)));
            
            sb5[0].AddContinueLine(500);

            sb5[0].AddStates((PortNumber.XAxis, new RelativeValue("XPosition", 15, RelativeValue.RelativeSign.Plus)),
                             (PortNumber.YAxis, new RelativeValue("YPosition", 40, RelativeValue.RelativeSign.Plus)));

            sb5[0].AddContinueLine(500);

            sb5[0].AddStates((PortNumber.XAxis, new RelativeValue("XPosition", 0, RelativeValue.RelativeSign.Plus)),
                             (PortNumber.YAxis, new RelativeValue("YPosition", 0, RelativeValue.RelativeSign.Plus)));


            #endregion

            //#region [  SetterBoard 6  ]

            //var sb6 = new SetterBoard();

            //sb6.Identifier = "지그재그로 움직이기";

            //sb6.AddSetterProperties(PortNumber.Color, "Color", "색깔", 1, true);
            //sb6.AddSetterProperties(PortNumber.Blink, "Blinking", "깜빡이는 정도", 0, true);
            //sb6.AddSetterProperties(PortNumber.Brightness, "Brightness", "밝기", 150, true);
            //sb6.AddSetterProperties(PortNumber.XAxis, "XPosition", "시작 X 좌표 위치", 156, true);
            //sb6.AddSetterProperties(PortNumber.YAxis, "YPosition", "시작 Y 좌표 위치", 30, true);

            //sb6.InitalizeValue = new List<ValueSetter>()
            //{
            //    new ValueSetter(PortNumber.Color, "Color"),
            //    new ValueSetter(PortNumber.Blink, "Blinking"),
            //    new ValueSetter(PortNumber.Brightness, "Brightness"),
            //    new ValueSetter(PortNumber.XAxis, "XPosition"),
            //    new ValueSetter(PortNumber.YAxis, "YPosition"),
            //};

            //sb6[0].AddStates((PortNumber.XAxis, new RelativeValue("XPosition", 50, RelativeValue.RelativeSign.Plus)),
            //    );

                
            //#endregion

            AddBoard(sb1);
            AddBoard(sb2);
            AddBoard(sb3);
            AddBoard(sb4);
            AddBoard(sb5);
        }

        public void AddBoard(SetterBoard sb)
        {
            GlobalViewModel.MainWindowViewModel.MediaItems.Add(new LightComponent(sb));
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            
        }

        int i = 1;
        int j = 10;
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (lvEvents.ItemsSource is ObservableCollection<Tuple<TimeSpan, object>> coll)
            {
                coll.Add((TimeSpan.FromSeconds(j), (object)$"효과 {i++}번째").ToTuple());

                j += 20;
            }
        }

        private void Tl_ItemRemoved(object sender, EventArgs e)
        {
            propGrid.SelectedObjects = null;
            lvEvents.ItemsSource = null;
        }

        List<ItemPosition> SavedItems { get; set; }

        private void Tl_ItemDeselected(object sender, EventArgs e)
        {
            propGrid.SelectedObjects = null;
            lvEvents.ItemsSource = null;
        }

        private void Tl_ItemSelected(object sender, EventArgs e)
        {
            if (tl.SelectedItem.Property != null)
            {
                propGrid.SelectedObjects = new object[] { tl.SelectedItem.Property };
                lvEvents.ItemsSource = tl.SelectedItem.KeyEvents;

                //tl.SelectedItem.KeyEvents[0].
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            pw = new PlayWindow();
            pw.Show();
            pw.ConnectTimeLine(tl);
            pw.ConnectPreview(preview);

            tl.FrameRate = FrameRate._60PFS;

            tl.AddTrack(SourceType.Sound, 1);
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

            mwViewModel.TimeLine = tl;

            this.DataContext = mwViewModel;

            topMenu.Items.Clear();
            topMenu.ItemsSource = mwViewModel.Menus;
            //lbMediaItem.ItemsSource = mwViewModel.MediaItemsView;

            CommandBindings.Add(new CommandBinding(mwViewModel.OpenFileCommand, mwViewModel.OpenFileExecuted));
            CommandBindings.Add(new CommandBinding(mwViewModel.OpenTemplateCommand, mwViewModel.OpenTemplateExecuted));
            CommandBindings.Add(new CommandBinding(mwViewModel.TemplateManageCommand, mwViewModel.TemplateManageExecuted));
            CommandBindings.Add(new CommandBinding(mwViewModel.EditTimeLineCommand, mwViewModel.EditTimeLineExecuted));
            CommandBindings.Add(new CommandBinding(mwViewModel.GetExternalSourceCommand, mwViewModel.GetExternalSourceExecuted));
            CommandBindings.Add(new CommandBinding(mwViewModel.ResetMovingLightCommand, mwViewModel.ResetMovingLightExecuted));
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
