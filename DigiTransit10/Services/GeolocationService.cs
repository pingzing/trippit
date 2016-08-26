using DigiTransit10.Helpers;
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
        Task<GenericResult<Geoposition>> GetCurrentLocationAsync();
    }

    public class GeolocationService : IGeolocationService
    {
        private readonly Geolocator _geolocator;

        public GeolocationService()
        {
            _geolocator = new Geolocator
            {
                ReportInterval = 10000 //once every 10 seconds should be sufficient. todo: revisit when we start implementing compass and realtime geotracking
            };            
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
                    return GenericResult<Geoposition>.Fail;
                }
            }
            else
            {
                //todo: pop up a reason explaining why it didn't work.
                return GenericResult<Geoposition>.Fail;
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
