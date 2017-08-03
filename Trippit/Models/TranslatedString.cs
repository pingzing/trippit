namespace Trippit.Models
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
