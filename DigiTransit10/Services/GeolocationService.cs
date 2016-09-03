using DigiTransit10.Helpers;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using static DigiTransit10.Services.GeolocationService;

namespace DigiTransit10.Services
{
    public interface IGeolocationService
    {
        Task<GenericResult<Geoposition>> GetCurrentLocationAsync();
        LiveGeolocationToken BeginLiveUpdates();
        event GeolocatorPositionChangedHandler PositionChanged;
        event CompassHeadingChangedHandler HeadingChanged; 
    }

    public class GeolocationService : IGeolocationService
    {
        private const uint InactiveUpdateInterval = 10000;        
        private const uint ActiveUpdateInterval = 1000;
        private volatile bool _isUpdatingLive = false;

        private readonly Geolocator _geolocator;
        private readonly Compass _compass;
        private LiveGeolocationToken _token;        

        public delegate void GeolocatorPositionChangedHandler(PositionChangedEventArgs args);
        public event GeolocatorPositionChangedHandler PositionChanged;

        public delegate void CompassHeadingChangedHandler(CompassReadingChangedEventArgs args);
        public event CompassHeadingChangedHandler HeadingChanged;

        public GeolocationService()
        {
            _geolocator = new Geolocator
            {
                ReportInterval = InactiveUpdateInterval 
            };

            _compass = Compass.GetDefault();
        }

        /// <summary>
        /// Gets the user's current Geoposition, and prompts for access if necessary. Returns null on failure.
        /// </summary>
        /// <returns></returns>
        public async Task<GenericResult<Geoposition>> GetCurrentLocationAsync()
        {
            if(await GetAccessStatus() == GeolocationAccessStatus.Allowed)
            {
                try
                {
                    return new GenericResult<Geoposition>(await _geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10)));
                }
                catch(Exception ex)
                {
                    //todo: log exception?
                    return GenericResult<Geoposition>.Fail;
                }
            }
            else
            {
                //todo: pop up a reason explaining why it didn't work.
                return GenericResult<Geoposition>.Fail;
            }
        }

        public LiveGeolocationToken BeginLiveUpdates()
        {
            if(_token != null)
            {
                EndLiveUpdates();
            }
            _isUpdatingLive = true;
            _geolocator.ReportInterval = ActiveUpdateInterval;
            _geolocator.PositionChanged += Geolocator_PositionChanged;            
            if(_compass != null)
            {
                _compass.ReadingChanged += Compass_ReadingChanged;
            }

            _token = new LiveGeolocationToken(this);            
            return _token;
        }

        public void EndLiveUpdates()
        {
            if (_isUpdatingLive)
            {                
                _geolocator.ReportInterval = InactiveUpdateInterval;
                _geolocator.PositionChanged -= Geolocator_PositionChanged;
                if(_compass != null)
                {
                    _compass.ReadingChanged -= Compass_ReadingChanged;
                }
                _isUpdatingLive = false;
                _token.Dispose(); //In case this method is called directly, we still need to handle disposing.
            }
        }

        private async Task<GeolocationAccessStatus> GetAccessStatus()
        {
            GeolocationAccessStatus accessStatus = GeolocationAccessStatus.Unspecified;
            TaskCompletionSource<bool> resultRetrieved = new TaskCompletionSource<bool>();

            DispatcherHelper.CheckBeginInvokeOnUI(
                () => Geolocator.RequestAccessAsync().AsTask()
                .ContinueWith(status => {
                    accessStatus = status.Result;
                    resultRetrieved.SetResult(true);
                }));

            await resultRetrieved.Task;
            return accessStatus;
        }

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => PositionChanged?.Invoke(args));
        }

        private void Compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => HeadingChanged?.Invoke(args));
        }               
    }
}
