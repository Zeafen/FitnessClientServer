using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.commands
{
    public class RelayCommand : ModifyCommandBase
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand() { }
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _canExecute = canExecute;
            _execute = execute ?? throw new ArgumentNullException(nameof(_execute));
        }

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public override bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public override void Execute(object parameter) => _execute(parameter);
    }
}
