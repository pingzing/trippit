using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiTransit10.Controls
{
    public sealed partial class AddOrEditFavoriteDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly ICustomFontService _fontService;

        public IEnumerable<IMapPoi> MapPlace { get; set; }

        private ObservableCollection<FavoriteIcon> _possibleIconsList = new ObservableCollection<FavoriteIcon>();
        public ObservableCollection<FavoriteIcon> PossibleIconsList
        {
            get { return _possibleIconsList; }
            set
            {
                if (_possibleIconsList != value)
                {
                    _possibleIconsList = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _selectedIconIndex;
        public int SelectedIconIndex
        {
            get { return -1; }
            set { _selectedIconIndex = -1; }
        }

        public AddOrEditFavoriteDialog()
        {
            this.InitializeComponent();
            this.Loaded += AddOrEditFavoriteDialog_Loaded;            
            _fontService = (ICustomFontService)ServiceLocator.Current.GetService(typeof(ICustomFontService));
        }        

        private async void AddOrEditFavoriteDialog_Loaded(object sender, RoutedEventArgs e)
        {
            SingleMap.Focus(FocusState.Programmatic); //prevent auto-focusing on the search box

            FontFamily hslFamily = null;
            FontFamily segoeFamily = null;
            TaskCompletionSource<bool> fontFamiliesFound = new TaskCompletionSource<bool>();

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                //apparently FontFamilies have to be constructed on the UI thread. Who knew?
                hslFamily = (FontFamily)App.Current.Resources[Constants.HslPiktoFrameFontFamilyKey];
                segoeFamily = new FontFamily(Constants.SymbolFontFamily);
                fontFamiliesFound.SetResult(true);
            });
            
            await fontFamiliesFound.Task;

            await Task.Delay(10); //give UI time to render before we go hunting for glyphs

            List<int> fontInts = (await _fontService.GetFontGlyphsAsync(hslFamily.Source)).ToList();
            foreach (int value in fontInts)
            {
                var icon = new FavoriteIcon
                {
                    FontFamily = hslFamily,
                    Glyph = ((char)(int.Parse(value.ToString("X"), System.Globalization.NumberStyles.HexNumber))).ToString()
                };
                PossibleIconsList.AddSorted(icon, (x1, x2) => x1.Glyph.CompareTo(x2.Glyph));                
            }

            Array enumsValues = Enum.GetValues(typeof(Symbol));
            foreach (var value in enumsValues)
            {
                int currentValue = (int)value;
                var icon = new FavoriteIcon
                {
                    FontFamily = segoeFamily,
                    Glyph = ((char)(int.Parse(currentValue.ToString("X"), System.Globalization.NumberStyles.HexNumber))).ToString()
                };
                PossibleIconsList.Add(icon);                
            }                        
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
