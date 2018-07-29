using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Delight.Common
{
    public static class MenuCommands
    {
        #region [  File Tab  ]

        public static RoutedCommand OpenFileCommand { get; }

        public static RoutedCommand OpenProjectCommand { get; }

        public static RoutedCommand NewProjectCommand { get; }

        public static RoutedCommand SaveCommand { get; }

        public static RoutedCommand SaveAsCommand { get; }

        public static RoutedCommand ExportCommand { get; }

        public static string ExportInputGestureText { get; set; } = "Ctrl+E";

        public static RoutedCommand ExitCommand { get; }

        #endregion

        #region [  Edit Tab  ]

        public static RoutedCommand UndoCommand { get; }

        public static RoutedCommand RedoCommand { get; }

        #endregion

        #region [  Help Tab  ]

        public static RoutedCommand ViewHelpCommand { get; }

        public static RoutedCommand ViewTutorialCommand { get; }

        public static RoutedCommand ViewInfoCommand { get; }

        #endregion

        static MenuCommands()
        {
            OpenFileCommand = new RoutedCommand("OpenFile", typeof(MenuCommands));
            OpenProjectCommand = new RoutedCommand("OpenProject", typeof(MenuCommands));
            NewProjectCommand = new RoutedCommand("NewProject", typeof(MenuCommands));

            SaveCommand = new RoutedCommand("Save", typeof(MenuCommands));
            SaveAsCommand = new RoutedCommand("SaveAs", typeof(MenuCommands));

            ExportCommand = new RoutedCommand("Export", typeof(MenuCommands));
            ExitCommand = new RoutedCommand("Exit", typeof(MenuCommands));

            // ====================

            UndoCommand = new RoutedCommand("Undo", typeof(MenuCommands));
            RedoCommand = new RoutedCommand("Redo", typeof(MenuCommands));

            // ====================

            ViewHelpCommand = new RoutedCommand("ViewHelp", typeof(MenuCommands));
            ViewTutorialCommand = new RoutedCommand("ViewTutorial", typeof(MenuCommands));
            ViewInfoCommand = new RoutedCommand("ViewInfo", typeof(MenuCommands));
        }
    }
}
