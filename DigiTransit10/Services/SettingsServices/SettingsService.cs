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
                var theme = DeviceTypeHelper.GetDeviceFormFactorType() == DeviceFormFactorType.Phone
                    ? ApplicationTheme.Dark
                    : ApplicationTheme.Light;
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

        //Serialize as just a list of places to save space, but expose as dictionary with names as keys
        private List<IFavorite> _favorites;
        public List<IFavorite> Favorites
        {
            get
            {
                if (_favorites == null)
                {
                    string serialized = _helper.Read<string>(nameof(Favorites), "", SettingsStrategies.Roam);
                    if (String.IsNullOrEmpty(serialized) || serialized == "null")
                    {
                        _favorites = new List<IFavorite>();
                        return _favorites;
                    }
                    else
                    {
                        _favorites = JsonConvert.DeserializeObject<List<IFavorite>>(serialized, new JsonSerializerSettings {
                            TypeNameHandling = TypeNameHandling.Objects,
                            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                        });
                        return _favorites;
                    }
                }
                else
                {
                    return _favorites;
                }
            }
            set
            {
                _favorites = value;
                FlushFavoritesToStorage();
            }
        }        

        /// <summary>
        /// Flushes the in-memory list of <see cref="IFavorite"/>s in Settings to disk.
        /// </summary>
        public void FlushFavoritesToStorage()
        {            
            _helper.Write(nameof(Favorites), JsonConvert.SerializeObject(_favorites,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects}), 
                SettingsStrategies.Roam);
        }

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

