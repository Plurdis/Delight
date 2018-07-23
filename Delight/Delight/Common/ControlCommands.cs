using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Delight.Common
{
    public static class ControlCommands
    {
        public static RoutedCommand PlayCommand { get; }

        static ControlCommands()
        {
            PlayCommand = new RoutedCommand("PlayCommand", typeof(ControlCommands));
        }
    }
}
