using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiTransit10.Controls
{
    public enum AddOrEditDialogType { Add, Edit }

    public sealed partial class AddOrEditFavoriteDialog : ContentDialog, INotifyPropertyChanged
    {

        private readonly ICustomFontService _fontService;
        private AddOrEditDialogType _dialogType;
        private string _editDialogIconGlyph;
        private string _editDialogIconFont;
        private FavoriteRoute _favoriteRoute = null; //only non-null if we're working with a FavoriteRoute
        private FavoritePlace _favoritePlace = null; //only non-null if we're wokring with an IPlace

        public IFavorite ResultFavorite { get; private set; }

        public IEnumerable<IMapPoi> MapPlace { get; set; }

        private ObservableCollection<FavoriteIcon> _possibleIconsList = null;
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

        private IPlace _searchBoxPlace;
        public IPlace SearchBoxPlace
        {
            get { return _searchBoxPlace; }
            set
            {
                if (_searchBoxPlace != value)
                {
                    _searchBoxPlace = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSaveButtonEnabled));
                }
            }
        }

        private string _nameText;
        public string NameText
        {
            get { return _nameText; }
            set
            {
                if (_nameText != value)
                {
                    _nameText = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSaveButtonEnabled));
                }
            }
        }

        private int _selectedIconIndex;
        public int SelectedIconIndex
        {
            get { return _selectedIconIndex; }
            set
            {
                if (_selectedIconIndex != value)
                {
                    _selectedIconIndex = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSaveButtonEnabled));
                }
            }
        }

        public bool IsSaveButtonEnabled => !String.IsNullOrWhiteSpace(NameText)
                                            && SearchBoxPlace != null
                                            && SearchBoxPlace.Type != ModelEnums.PlaceType.NameOnly
                                            && SearchBoxPlace.Type != ModelEnums.PlaceType.UserCurrentLocation
                                            && SelectedIconIndex != -1;

        //todo: replace this with a converter in the view
        public bool IsAddNewDialog => _dialogType == AddOrEditDialogType.Add;

        public AddOrEditFavoriteDialog()
        {
            this.InitializeComponent();
            this.Loaded += AddOrEditFavoriteDialog_Loaded;
            _fontService = (ICustomFontService)ServiceLocator.Current.GetService(typeof(ICustomFontService));
            _dialogType = AddOrEditDialogType.Add;
        }

        public AddOrEditFavoriteDialog(IFavorite favoriteToEdit) : this()
        {
            _dialogType = AddOrEditDialogType.Edit;
            var favoriteAsPlace = favoriteToEdit as IPlace;
            if (favoriteAsPlace != null)
            {
                SearchBoxPlace = favoriteAsPlace;
            }
            NameText = favoriteToEdit.UserChosenName;
            _editDialogIconGlyph = favoriteToEdit.FontIconGlyph;
            _editDialogIconFont = favoriteToEdit.IconFontFace;
            _favoriteRoute = favoriteToEdit as FavoriteRoute;
            _favoritePlace = favoriteToEdit as FavoritePlace;
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

            List<FavoriteIcon> iconsList = new List<FavoriteIcon>();

            List<int> fontInts = (await _fontService.GetFontGlyphsAsync(hslFamily.Source)).ToList();
            foreach (int value in fontInts)
            {
                var icon = new FavoriteIcon
                {
                    FontFamily = hslFamily,
                    Glyph = ((char)(int.Parse(value.ToString("X"), System.Globalization.NumberStyles.HexNumber))).ToString()
                };
                iconsList.Add(icon);
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
                iconsList.Add(icon);
            }

            PossibleIconsList = new ObservableCollection<FavoriteIcon>(iconsList);

            // The SelectedIconIndex binding won't update correctly until the GridView has actually 
            // realized at least one element, so let's wait here for a moment to make sure that it's 
            // ready to go.
            await Task.Delay(100);

            if (!String.IsNullOrWhiteSpace(_editDialogIconFont)
                && !String.IsNullOrWhiteSpace(_editDialogIconGlyph))
            {
                SelectedIconIndex = PossibleIconsList.IndexOf
                (
                    PossibleIconsList.FirstOrDefault(x => x.FontFamily.Source == _editDialogIconFont && x.Glyph == _editDialogIconGlyph)
                );
            }

            if (SelectedIconIndex == -1)
            {
                SelectedIconIndex = 0; //In case we didn't find it, default to the first icon.
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (_dialogType == AddOrEditDialogType.Edit)
            {
                if (_favoritePlace != null)
                {
                    ResultFavorite = new FavoritePlace
                    {
                        Confidence = null,
                        FavoriteId = _favoritePlace.FavoriteId,
                        FontIconGlyph = PossibleIconsList[SelectedIconIndex].Glyph,
                        IconFontFace = PossibleIconsList[SelectedIconIndex].FontFamily.Source,
                        Id = null,
                        Lat = _favoritePlace.Lat,
                        Lon = _favoritePlace.Lon,
                        Name = _favoritePlace.Name,
                        UserChosenName = NameText,
                        Type = ModelEnums.PlaceType.FavoritePlace
                    };
                }
                else if (_favoriteRoute != null)
                {
                    ResultFavorite = new FavoriteRoute
                    {
                        FavoriteId = _favoriteRoute.FavoriteId,
                        FontIconGlyph = PossibleIconsList[SelectedIconIndex].Glyph,
                        IconFontFace = PossibleIconsList[SelectedIconIndex].FontFamily.Source,
                        RouteGeometryStrings = _favoriteRoute.RouteGeometryStrings,
                        RoutePlaces = _favoriteRoute.RoutePlaces,
                        UserChosenName = NameText
                    };
                }
            }
            else if (_dialogType == AddOrEditDialogType.Add)
            {
                ResultFavorite = new FavoritePlace
                {
                    Confidence = null,
                    FavoriteId = Guid.NewGuid(),
                    FontIconGlyph = PossibleIconsList[SelectedIconIndex].Glyph,
                    IconFontFace = PossibleIconsList[SelectedIconIndex].FontFamily.Source,
                    Id = null,
                    Lat = SearchBoxPlace.Lat,
                    Lon = SearchBoxPlace.Lon,
                    Name = SearchBoxPlace.Name,
                    UserChosenName = NameText,
                    Type = ModelEnums.PlaceType.FavoritePlace,
                };
            }
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ResultFavorite = null;
            this.Hide();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void IconsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView grid = (GridView)sender;
            var item = e.AddedItems.FirstOrDefault();
            if (item != null)
            {
                grid.ScrollIntoView(item);
            }
        }
    }
}
