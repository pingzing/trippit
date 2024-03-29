﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Trippit.Models.ApiModels;
using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.Models
{
    public class TransitTrafficAlert : ITransitLine
    {
        // We use this for a "null" URL because attempting to bind to something that's not a valid URL causes crashes.
        private static readonly Uri DummyUrl = new Uri("https://dummyurl");

        public string Id { get; set; }
        public TranslatedString HeaderText { get; set; }
        public TranslatedString DescriptionText { get; set; }
        public Uri Url { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string AffectedLineId { get; set; }
        public string AffectedLineShortName { get; set; }
        public string AffectedLineLongName { get; set; }
        public ApiMode AffectedLineMode { get; set; }
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
            Language language = LanguageEnum.LanguageCodeToLanuage(requestedLanguage.Substring(0, 2));
            if (result.AlertHeaderTextTranslations != null && result.AlertHeaderTextTranslations.Any())
            {
                HeaderText = result.AlertHeaderTextTranslations
                    .Where(x => x.LanguageAsEnum == language)
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
                    .Where(x => x.LanguageAsEnum == language)
                    .Select(x => new TranslatedString
                    {
                        Language = x.LanguageAsEnum,
                        ShortLanguageCode = x.Language,
                        Text = x.Text
                    })
                    .FirstOrDefault();
            }

            if (result.AlertUrlTranslations?.Any() == true)
            {
                string localizedUrl = result.AlertUrlTranslations.Where(x => x.LanguageAsEnum == language)
                     .Select(x => new TranslatedString
                     {
                         Language = x.LanguageAsEnum,
                         ShortLanguageCode = x.Language,
                         Text = x.Text
                     })
                     .FirstOrDefault()?.Text;
                if (localizedUrl == null)
                {
                    Url = DummyUrl;
                }
                else 
                {
                    bool validUrl = Uri.TryCreate(localizedUrl, UriKind.Absolute, out Uri localizedUri);
                    Url = validUrl ? localizedUri : DummyUrl;
                }
            }
            else
            {
                bool validUrl = Uri.TryCreate(result.AlertUrl, UriKind.Absolute, out Uri parsedUri);
                Url = validUrl ? parsedUri : DummyUrl;
            }

            if (result.EffectiveStartDate != null)
            {
                StartDate = DateTimeOffset.FromUnixTimeSeconds(result.EffectiveStartDate.Value);
            }

            if (result.EffectiveEndDate != null)
            {
                EndDate = DateTimeOffset.FromUnixTimeSeconds(result.EffectiveEndDate.Value);
            }

            AffectedLineId = result.Route?.GtfsId;
            AffectedLineShortName = result.Route?.ShortName;
            AffectedLineLongName = result.Route?.LongName;
            AffectedLineMode = result.Route?.Mode ?? ApiMode.Bus;

            AffectedStopId = result.Route?.GtfsId;
        }

        // ITransitLine implementation. All proxies to differently-named properties.

        /// <summary>
        /// alias for AffectedLineMode.
        /// </summary>
        public ApiMode TransitMode
        {
            get { return AffectedLineMode; }
            set { AffectedLineMode = value; }
        }

        /// <summary>
        /// Aliass for AffectedLineId.
        /// </summary>
        public string GtfsId
        {
            get { return AffectedLineId; }
            set { AffectedLineId = value; }
        }
        
        /// <summary>
        /// Alias for AffectedLineShortName.
        /// </summary>
        public string ShortName
        {
            get { return AffectedLineShortName; }
            set { AffectedLineShortName = value; }
        }

        /// <summary>
        /// Alias for AffectedLineLongName.
        /// </summary>
        public string LongName
        {
            get { return AffectedLineLongName; }
            set { AffectedLineLongName = value; }
        }
    }
}
