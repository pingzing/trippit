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

        private WalkingAmount _selectedWalkingAmount;
        public WalkingAmount SelectedWalkingAmount
        {
            get { return _selectedWalkingAmount; }
            set { Set(ref _selectedWalkingAmount, value); }
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

        private WalkingSpeed _selectedWalkingSpeed;
        public WalkingSpeed SelectedWalkingSpeed
        {
            get { return _selectedWalkingSpeed; }
            set { Set(ref _selectedWalkingSpeed, value); }
        }


        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;                    
        }

        public RelayCommand CycleThemeCommand => new RelayCommand(CycleTheme);
        private void CycleTheme()
        {
            var currTheme = _settingsService.AppTheme;            
            if (currTheme == ApplicationTheme.Light)
            {
                _settingsService.AppTheme = ApplicationTheme.Dark;
            }
            if (currTheme == ApplicationTheme.Dark)
            {
                _settingsService.AppTheme = ApplicationTheme.Light;
            }
            System.Diagnostics.Debug.WriteLine("Current theme is now:" + _settingsService.AppTheme);
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {


            return base.OnNavigatedToAsync(parameter, mode, state);
        }
    }
}
