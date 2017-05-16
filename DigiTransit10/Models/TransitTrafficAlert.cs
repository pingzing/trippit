using System;
using System.Collections.Generic;
using DigiTransit10.Models.ApiModels;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace DigiTransit10.Models
{
    public class TransitTrafficAlert
    {
        // We use this for a "null" URL because attempting to bind to something that's not a valid URL causes crashes.
        private const string DummyUrl = "https://dummyurl";

        public string Id { get; set; }
        public TranslatedString HeaderText { get; set; }
        public TranslatedString DescriptionText { get; set; }
        public string Url { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string AffectedLineId { get; set; }
        public string AffectedStopId { get; set; }

        [JsonIgnore]
        public bool IsDummyUrl => Url == DummyUrl;

        [JsonIgnore]
        public string StartDateAsLocalTimeString
        {
            get
            {
                if (StartDate != null)
                {
                    return StartDate.Value.LocalDateTime.ToString("t", CultureInfo.CurrentUICulture);
                }
                else
                {
                    return null;
                }
            }
        }

        [JsonIgnore]
        public string EndDateAsLocalTimeString
        {
            get
            {
                if (EndDate != null)
                {
                    return EndDate.Value.LocalDateTime.ToString("t", CultureInfo.CurrentUICulture);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// This constructor exists just to make the serializers happy.
        /// </summary>
        public TransitTrafficAlert() { }

        public TransitTrafficAlert(ApiAlert result, string requestedLanguage)
        {
            Id = result.Id;
            Language requested = LanguageEnum.LanguageCodeToLanuage(requestedLanguage.Substring(0, 2));
            if (result.AlertHeaderTextTranslations != null && result.AlertHeaderTextTranslations.Any())
            {
                HeaderText = result.AlertHeaderTextTranslations
                    .Where(x => x.LanguageAsEnum == requested)
                    .Select(x => new TranslatedString
                    {
                        Language = x.LanguageAsEnum,
                        ShortLanguageCode = x.Language,
                        Text = x.Text
                    })
                    .FirstOrDefault();
            }

            if (result.AlertDescriptionTextTranslations != null && result.AlertDescriptionTextTranslations.Any())
            {
                DescriptionText = result.AlertDescriptionTextTranslations
                    .Where(x => x.LanguageAsEnum == requested)
                    .Select(x => new TranslatedString
                    {
                        Language = x.LanguageAsEnum,
                        ShortLanguageCode = x.Language,
                        Text = x.Text
                    })
                    .FirstOrDefault();
            }

            Url = result.AlertUrl ?? DummyUrl;
            if (result.EffectiveStartDate != null)
            {
                StartDate = DateTimeOffset.FromUnixTimeSeconds(result.EffectiveStartDate.Value);
            }

            if (result.EffectiveEndDate != null)
            {
                EndDate = DateTimeOffset.FromUnixTimeSeconds(result.EffectiveEndDate.Value);
            }

            AffectedLineId = result.Route.GtfsId;
            AffectedStopId = result.Route.GtfsId;
        }
    }
}
