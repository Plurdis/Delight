using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StagePainter.Common
{
    public static class MenuCommands
    {
        public static RoutedCommand ExitCommand { get; }
        public static RoutedCommand ExportCommand { get; }
        public static RoutedCommand OpenFileCommand { get; }

        static MenuCommands()
        {
            ExitCommand = new RoutedCommand("Exit", typeof(MenuCommands));
            ExportCommand = new RoutedCommand("Export", typeof(MenuCommands));
            OpenFileCommand = new RoutedCommand("OpenFile", typeof(MenuCommands));
        }
    }
}
