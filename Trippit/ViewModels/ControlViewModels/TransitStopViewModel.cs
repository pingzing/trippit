using DigiTransit10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class TransitStopViewModel : BindableBase
    {
        public TransitStop BackingStop { get; set; }
        public string NameAndCode
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(BackingStop.Code))
                {
                    return $"{BackingStop.Name}, {BackingStop.Code}";
                }
                else
                {
                    return BackingStop.Name;
                }
            }
        }
    }
}
