using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Delight.Common
{
    public class ToolBarCommand : ICommand
    {
        public ToolBarCommand(ImageSource source)
        {
            Image = source;
        }

        public ImageSource Image { get; set; }

        public event EventHandler CanExecuteChanged;

        public event EventHandler Executed;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Executed?.Invoke(this, null);
        }
    }
}
