using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Delight.Commands
{
    public class TrackCommands
    {
        public static RoutedCommand DeleteCommand { get; }

        public static RoutedCommand AddCommand { get; }

        static TrackCommands()
        {
            DeleteCommand = new RoutedCommand("DeleteCommand", typeof(TrackCommands));
            AddCommand = new RoutedCommand("AddCommand", typeof(TrackCommands));
        }
    }
}
