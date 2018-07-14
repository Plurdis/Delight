using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Delight.Common
{
    public static class CommandManager
    {
        static CommandBindingCollection CommandBindings;

        static CommandManager()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            
            CommandBindings = mw.CommandBindings;
        }

        public static void Init()
        {
            // Fake Method
        }

        public static void AddKeyGesture(string name, KeyGesture keyGesture)
        {
            try
            {
                RoutedCommand itm = commands[name];
                itm.InputGestures.Add(keyGesture);
            }
            catch (Exception)
            {
            }
            
        }

        static Dictionary<string, RoutedCommand> commands = new Dictionary<string, RoutedCommand>();

        public static RoutedCommand GetCommandByName(string name)
        {
            return commands[name];
        }

        public static void AddCommand(string name, ExecutedRoutedEventHandler executed, List<InputGesture> inputGestures)
        {
            if (commands.ContainsKey(name))
            {
                return;
            }
            RoutedCommand command = new RoutedCommand(name, typeof(CommandManager));
            commands[name] = command;

            var commandBinding = new CommandBinding(command, executed);
            inputGestures.ForEach(i => command.InputGestures.Add(i));

            CommandBindings.Add(commandBinding);
        }

        public static void AddCommand(string name, ExecutedRoutedEventHandler executed, InputGesture inputGesture)
        {
            AddCommand(name, executed, new List<InputGesture>() { inputGesture });
        }
    }
}
