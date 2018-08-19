using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Delight.Commands
{
    public static class DebugCommands
    {
        public static RoutedCommand PlayWindowVisibleCommand { get; }

        public static RoutedCommand UnityPreviewVisibleCommand { get; }

        static DebugCommands()
        {
            PlayWindowVisibleCommand = new RoutedCommand("PlayWindowVisibleCommand",typeof(DebugCommands));
            UnityPreviewVisibleCommand = new RoutedCommand("UnityPreviewVisibleCommand", typeof(DebugCommands));
        }
    }
}
