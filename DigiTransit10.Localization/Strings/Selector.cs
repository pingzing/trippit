using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Localization.Strings
{
    public class Selector
    {                        
        static Selector()
        {
            var ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
            ResourceManager manager = new ResourceManager("DigiTransit10.Localization.AppResources", typeof(Selector).GetTypeInfo().Assembly);            
        }

    }
}
