using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Delight.Commands
{
    public static class TrackItemCommands
    {
        public static RoutedCommand DeleteCommand { get; }

        static TrackItemCommands()
        {
            DeleteCommand = new RoutedCommand("DeleteCommand", typeof(TrackItemCommands));
        }
    }
}
