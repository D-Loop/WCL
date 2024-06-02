using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace WCL.Helpers
{
    public class Command : ICommand
    {
        private readonly Action _execute;

        public Command(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
    }

}
