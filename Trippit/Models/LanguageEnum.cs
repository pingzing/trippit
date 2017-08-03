namespace Trippit.Models
{
    public enum Language
    {
        English,
        Finnish,
        Swedish            
    }

    public static class LanguageEnum
    {
        public static Language LanguageCodeToLanuage(string str)
        {
            switch (str)
            {
                case "fi":
                    return Language.Finnish;
                case "sv":
                    return Language.Swedish;
                case "en":
                default:
                    return Language.English;
            }
        }
    }
}
