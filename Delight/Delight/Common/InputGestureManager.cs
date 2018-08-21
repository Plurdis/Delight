using Delight.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Delight.Common
{
    public static class InputGestureManager
    {

        static InputGestureManager()
        {
            RegisterCommand(MenuCommands.NewProjectCommand, new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift));

            RegisterCommand(MenuCommands.OpenProjectCommand, new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift));
            RegisterCommand(MenuCommands.OpenFileCommand, new KeyGesture(Key.O, ModifierKeys.Control));

            RegisterCommand(MenuCommands.SaveAsCommand, new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
            RegisterCommand(MenuCommands.SaveCommand, new KeyGesture(Key.S, ModifierKeys.Control));

            RegisterCommand(MenuCommands.ExportCommand, new KeyGesture(Key.E, ModifierKeys.Control));


            RegisterCommand(MenuCommands.ViewInfoCommand, new KeyGesture(Key.H, ModifierKeys.Control | ModifierKeys.Shift));

//#if DEBUG
            RegisterCommand(DebugCommands.PlayWindowVisibleCommand, new KeyGesture(Key.B, ModifierKeys.Control));
            RegisterCommand(DebugCommands.UnityPreviewVisibleCommand, new KeyGesture(Key.G, ModifierKeys.Control));
//#endif
        }

        static Dictionary<string, RoutedCommand> commands = new Dictionary<string, RoutedCommand>();

        public static bool RegisterCommand(RoutedCommand command)
        {
            if (commands.ContainsKey(command.Name))
            {
                return false;
            }
            commands[command.Name] = command;
            return true;
        }

        public static bool RegisterCommand(RoutedCommand command, KeyGesture keyGesture)
        {
            bool b = RegisterCommand(command);
            commands[command.Name].InputGestures.Add(keyGesture);

            return b;
        }

        public static bool RegisterCommand(RoutedCommand command, List<KeyGesture> keyGesture)
        {
            bool b = RegisterCommand(command);

            keyGesture.ForEach((i) =>
            {
                commands[command.Name].InputGestures.Add(i);
            });

            return b;
        }

        public static void Init()
        {
            // Fake Method
        }
    }
}
