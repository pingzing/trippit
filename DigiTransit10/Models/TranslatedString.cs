using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigiTransit10.Models;

namespace DigiTransit10.Models
{
    public class TranslatedString
    {
        public Language Language { get; set; }
        public string ShortLanguageCode { get; set; }
        public string Text { get; set; }

        public TranslatedString()
        {

        }
    }
}
