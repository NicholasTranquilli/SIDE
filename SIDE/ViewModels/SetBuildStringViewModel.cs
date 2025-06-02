using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIDE.ViewModels
{
    public class SetBuildStringViewModel : ViewModelBase
    {
        public string TerminalString { get; set; }
        public SetBuildStringViewModel(string initialValue)
        {
            TerminalString = initialValue;
        }
    }
}
