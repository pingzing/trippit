using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using DigiTransit10.Services.SettingsServices;
using System.Collections.ObjectModel;
using Template10.Mvvm;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using System;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using DigiTransit10.Helpers;

namespace DigiTransit10.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {

        private readonly SettingsService _settingsService;

        public IList<WalkingAmount> WalkingAmounts => Constants.WalkingAmounts;
        
        public WalkingAmount SelectedWalkingAmount
        {
            get
            {
                return WalkingAmounts.FirstOrDefault(x => x.AmountType == _settingsService.PreferredWalkingAmount)
                    ?? Constants.DefaultWalkingAmount;
            }
            set
            {
                if (_settingsService.PreferredWalkingAmount != value.AmountType)
                {
                    _settingsService.PreferredWalkingAmount = value.AmountType;
                    RaisePropertyChanged(nameof(SelectedWalkingAmount));
                }
            }
        }

        public IList<WalkingSpeed> WalkingSpeeds => Constants.WalkingSpeeds;
        
        public WalkingSpeed SelectedWalkingSpeed
        {
            get
            {
                return WalkingSpeeds.FirstOrDefault(x => x.SpeedType == _settingsService.PreferredWalkingSpeed)
                    ?? Constants.DefaultWalkingSpeed;
            }
            set
            {
                if (_settingsService.PreferredWalkingSpeed != value.SpeedType)
                {
                    _settingsService.PreferredWalkingSpeed = value.SpeedType;
                    RaisePropertyChanged(nameof(SelectedWalkingSpeed));
                }
            }
        }

        public List<string> ThemeOptions = new List<string>
        {
            AppResources.DarkThemeName,
            AppResources.LightThemeName,
            AppResources.SystemSettingThemeName
        };

        public List<int> PinnedFavoritesOptions = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9
        };


        public ElementTheme? SelectedTheme
        {
            get { return _settingsService.AppTheme; }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (_settingsService.AppTheme != value.Value)
                {
                    _settingsService.AppTheme = value.Value;
                    RaisePropertyChanged(nameof(SelectedTheme));
                }
            }
        }

        public int SelectedFavoritePlacesDisplayNumber
        {
            get { return _settingsService.PinnedFavoritePlacesDisplayNumber; }
            set
            {
                if (_settingsService.PinnedFavoritePlacesDisplayNumber != value)
                {
                    _settingsService.PinnedFavoritePlacesDisplayNumber = value;
                    RaisePropertyChanged(nameof(SelectedFavoritePlacesDisplayNumber));
                }
            }
        }

        public IPlace SelectedFromPlace
        {
            get { return _settingsService.PreferredFromPlace; }
            set
            {
                IPlace currValue = _settingsService.PreferredFromPlace;
                if (currValue == null || !currValue.Equals(value))
                {
                    _settingsService.PreferredFromPlace = value;
                    RaisePropertyChanged(nameof(SelectedFromPlace));
                }
            }
        }

        public IPlace SelectedToPlace
        {
            get { return _settingsService.PreferredToPlace; }
            set
            {
                IPlace currValue = _settingsService.PreferredToPlace;
                if(currValue == null || !currValue.Equals(value))
                {
                    _settingsService.PreferredToPlace = value;
                    RaisePropertyChanged(nameof(SelectedToPlace));
                }
            }
        }

        public bool IsAnalyticsEnabled
        {
            get { return _settingsService.IsAnalyticsEnabled; }
            set
            {
                _settingsService.IsAnalyticsEnabled = value;
                RaisePropertyChanged(nameof(IsAnalyticsEnabled));
            }
        }

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;            
        }     
    }
}
