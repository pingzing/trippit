using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using System.Collections.ObjectModel;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {

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

        public SettingsViewModel()
        {
            // Get defaults from settings

            // todo
            
        }
    }
}
