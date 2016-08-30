using System;

namespace DigiTransit10.Services
{
    public class LiveGeolocationToken : IDisposable
    {
        private GeolocationService _ownerService;

        public LiveGeolocationToken(GeolocationService owner)
        {
            _ownerService = owner;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _ownerService.EndLiveUpdates();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
