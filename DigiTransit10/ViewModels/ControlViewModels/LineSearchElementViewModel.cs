using DigiTransit10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class LineSearchElementViewModel : BindableBase
    {
        public TransitLine BackingLine { get; set; }

        public LineSearchElementViewModel()
        {
            //Designer is now happy
        }
    }
}
