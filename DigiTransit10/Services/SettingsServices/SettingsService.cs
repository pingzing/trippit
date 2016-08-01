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

        /// <summary>
        /// Access a read-only collection of the user's IFavorites. To Add or Remove, call the AddFavorite() or RemoveFavorite() methods.
        /// </summary>
        private List<IFavorite> _favorites;
        public IReadOnlyList<IFavorite> Favorites
        {
            get
            {
                if (_favorites == null)
                {
                    string serialized = _helper.Read<string>(nameof(Favorites), "", SettingsStrategies.Roam);
                    if (String.IsNullOrEmpty(serialized) || serialized == "null")
                    {
                        _favorites = new List<IFavorite>();
                        return _favorites.AsReadOnly();
                    }
                    else
                    {
                        _favorites = JsonConvert.DeserializeObject<List<IFavorite>>(serialized, new JsonSerializerSettings {
                            TypeNameHandling = TypeNameHandling.Objects,
                            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                        });
                        return _favorites.AsReadOnly();
                    }
                }
                else
                {
                    return _favorites;
                }
            }
            set
            {
                _favorites = value.ToList();
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

        /// <summary>
        /// Add favorite to backing list behind <see cref="Favorites"/>, and flushes the changed favorites list to storage.
        /// </summary>
        /// <param name="newFavorite"></param>
        public void AddFavorite(IFavorite newFavorite)
        {
            _favorites.Add(newFavorite);
            Messenger.Default.Send(new MessageTypes.FavoritesChangedMessage(new List<IFavorite> { newFavorite }, null));
            FlushFavoritesToStorage();
        }

        public bool RemoveFavorite(IFavorite toRemove)
        {
            bool success =_favorites.Remove(toRemove);
            if (success)
            {
                Messenger.Default.Send(new MessageTypes.FavoritesChangedMessage(null, new List<IFavorite> { toRemove }));
                FlushFavoritesToStorage();
            }
            return success;
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

