using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace DigiTransit10.Services
{
    public interface IGeolocationService
    {
        Task<Geoposition> GetCurrentLocation();
    }

    public class GeolocationService : IGeolocationService
    {
        private Geolocator _geolocator;

        public GeolocationService()
        {
            _geolocator = new Geolocator();
        }

        /// <summary>
        /// Gets the user's current Geoposition, and prompts for access if necessary. Returns null on failure.
        /// </summary>
        /// <returns></returns>
        public async Task<Geoposition> GetCurrentLocation()
        {
            if(await GetAccessStatus() == GeolocationAccessStatus.Allowed)
            {
                return await _geolocator.GetGeopositionAsync();
            }
            else
            {
                //todo: pop up a reason explaining why it didn't work.
                return null;
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
    }
}
