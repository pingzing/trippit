using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels
{
    public class ViewModelBaseEx : ViewModelBase
    {
        /// <summary>
        /// The one in ViewModelBase doesn't seem to retireve properly, for some reason.
        /// </summary>
        public new StateItems SessionState => BootStrapper.Current.SessionState;
    }
}
