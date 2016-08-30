using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using System.Collections.Generic;
using Template10.Services.SettingsService;
using Newtonsoft.Json;
using Windows.Globalization;
using Windows.System.UserProfile;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;

namespace DigiTransit10.Services.SettingsServices
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new SettingsHelper();
        }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value);
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        private List<IFavorite> _pinnedFavorites;
        public IReadOnlyList<IFavorite> PinnedFavorites
        {
            get
            {
                if (_pinnedFavorites == null)
                {
                    string serialized = _helper.Read(nameof(PinnedFavorites), "", SettingsStrategies.Roam);
                    if (String.IsNullOrEmpty(serialized) || serialized == "null")
                    {
                        _pinnedFavorites = new List<IFavorite>();
                        return _pinnedFavorites.AsReadOnly();
                    }
                    else
                    {
                        _pinnedFavorites = JsonConvert.DeserializeObject<List<IFavorite>>(serialized,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Objects,
                                TypeNameAssemblyFormat =
                                    System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                            });
                        return _pinnedFavorites.AsReadOnly();
                    }
                }
                else
                {
                    return _pinnedFavorites.AsReadOnly();
                }
            }
            set
            {
                _pinnedFavorites = value.ToList();
                _helper.Write(nameof(PinnedFavorites), JsonConvert.SerializeObject(_pinnedFavorites,
                        new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects}),
                    SettingsStrategies.Roam);
            }
        }

        public void AddPinnedFavorite(IFavorite newPinned)
        {
            _pinnedFavorites.Add(newPinned);
            FlushPinnedFavoritesToStorage();
        }

        public void RemovePinnedFavorite(IFavorite removedPinned)
        {
            _pinnedFavorites.Remove(removedPinned);
            FlushPinnedFavoritesToStorage();
        }

        public void FlushPinnedFavoritesToStorage()
        {
            _helper.Write(nameof(PinnedFavorites), JsonConvert.SerializeObject(_pinnedFavorites,
                        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects }),
                    SettingsStrategies.Roam);
        }

        /// <summary>
        /// The number of favorites to show in the Pinned Favorites on MainPage. Defaults to 3.
        /// </summary>
        public int PinnedFavoritePlacesDisplayNumber
        {
            //local, because this is going to differ greatly on display size, and roaming it doesn't make a lot of sense.
            get { return _helper.Read(nameof(PinnedFavorites), 3, SettingsStrategies.Local); }
            set { _helper.Write(nameof(PinnedFavorites), value, SettingsStrategies.Local); }
        }

        //todo: make this persist somehow
        public string CurrentLanguage
        {
            get
            {
                string langOverride = ApplicationLanguages.PrimaryLanguageOverride;
                if(String.IsNullOrWhiteSpace(langOverride))
                {
                    return ApplicationLanguages.Languages.FirstOrDefault() ?? "en-US";
                }
                else
                {
                    return ApplicationLanguages.PrimaryLanguageOverride;
                }
            }
            set
            {
                ApplicationLanguages.PrimaryLanguageOverride = value;
                //do something to reload values?
            }
        }
    }
}

