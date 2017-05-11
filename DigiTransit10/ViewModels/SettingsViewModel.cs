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

namespace DigiTransit10.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {

        private readonly SettingsService _settingsService;

        private ObservableCollection<WalkingAmount> _walkingAmounts = new ObservableCollection<WalkingAmount>
        {
            new WalkingAmount
            {
                AmountType = WalkingAmountType.BareMinimum,
                DisplayName = AppResources.WalkingAmount_0
            },
            new WalkingAmount
            {
                AmountType = WalkingAmountType.Some,
                DisplayName = AppResources.WalkingAmount_1
            },
            new WalkingAmount
            {
                AmountType = WalkingAmountType.Normal,
                DisplayName = AppResources.WalkingAmount_2
            },
            new WalkingAmount
            {
                AmountType = WalkingAmountType.Lots,
                DisplayName = AppResources.WalkingAmount_3
            },
            new WalkingAmount
            {
                AmountType = WalkingAmountType.Maximum,
                DisplayName = AppResources.WalkingAmount_4
            }
        };
        public ObservableCollection<WalkingAmount> WalkingAmounts
        {
            get { return _walkingAmounts; }
            set { Set(ref _walkingAmounts, value); }
        }
        
        public WalkingAmount SelectedWalkingAmount
        {
            get
            {
                return _walkingAmounts.FirstOrDefault(x => x.AmountType == _settingsService.PreferredWalkingAmount)
                    ?? _walkingAmounts[2];
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

        private ObservableCollection<WalkingSpeed> _walkingSpeeds = new ObservableCollection<WalkingSpeed>
        {
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Glacial,
                DisplayName = AppResources.WalkingSpeed_0,
                DisplaySubtitle = AppResources.WalkingSpeed_0_Subtitle
            },
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Slow,
                DisplayName = AppResources.WalkingSpeed_1,
                DisplaySubtitle = AppResources.WalkingSpeed_1_Subtitle
            },
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Normal,
                DisplayName = AppResources.WalkingSpeed_2,
                DisplaySubtitle = AppResources.WalkingSpeed_2_Subtitle
            },
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Fast,
                DisplayName = AppResources.WalkingSpeed_3,
                DisplaySubtitle = AppResources.WalkingSpeed_3_Subtitle
            },
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Breakneck,
                DisplayName = AppResources.WalkingSpeed_4,
                DisplaySubtitle = AppResources.WalkingSpeed_4_Subtitle
            }
        };
        public ObservableCollection<WalkingSpeed> WalkingSpeeds
        {
            get { return _walkingSpeeds; }
            set { Set(ref _walkingSpeeds, value); }
        }
        
        public WalkingSpeed SelectedWalkingSpeed
        {
            get
            {
                return _walkingSpeeds.FirstOrDefault(x => x.SpeedType == _settingsService.PreferredWalkingSpeed)
                    ?? _walkingSpeeds[2];
            }
            set
            {
                if (_settingsService.PreferredWalkingSpeed != value.SpeedType)
                {
                    _settingsService.PreferredWalkingSpeed = value.SpeedType;
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

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;            
        }     
    }
}
