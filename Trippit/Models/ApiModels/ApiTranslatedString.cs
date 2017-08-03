using Newtonsoft.Json;

namespace Trippit.Models.ApiModels
{
    public class ApiTranslatedString
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonIgnore]
        public Language LanguageAsEnum
        {
            get
            {
                return LanguageEnum.LanguageCodeToLanuage(Language);
            }
        }
    }
}
