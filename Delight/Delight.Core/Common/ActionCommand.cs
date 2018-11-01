using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Delight.Core.Common
{
    
    public class ActionCommand : ICommand
    {
        public ActionCommand(Action action)
        {
            Action = action;
        }
        public ActionCommand()
        {

        }

        [NonSerialized]
        private Action _action;

        public Action Action
        {
            get => _action;
            set => _action = value;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action?.Invoke();
        }
    }
}
