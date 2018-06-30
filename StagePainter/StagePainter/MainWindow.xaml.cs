using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
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
using StagePainter.Common;
using StagePainter.Core.Common;
using StagePainter.Windows;

using LocalCommandManager = StagePainter.Common.CommandManager;

namespace StagePainter
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
            InfoWindow wdw = new InfoWindow();
            wdw.ShowDialog();
#if DEBUG
            //img.Source = ImageCreator.GetWireFrame(200, 300, Brushes.Red);
#endif

            CommandBindings.Add(new CommandBinding(MenuCommands.ExitCommand, (s, e) =>  Environment.Exit(0), (s, e) => e.CanExecute = true));
            MenuCommands.ExitCommand.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
        }
        
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
