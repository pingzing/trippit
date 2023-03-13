using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Trippit.Helpers
{
    public static class DigiTransitApiKey
    {
        private static string _key = null;
        public static string Key 
        {
            get
            {
                if (_key == null)
                {
                    throw new ArgumentNullException(nameof(Key));
                }
                return _key;
            }
        }

        public static async Task InitKey()
        {
#if DEBUG
            StorageFile keyfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///digitransit-api-key-dev.txt"));
#else
            StorageFile keyfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///digitransit-api-key.txt"));
#endif
            string key = await FileIO.ReadTextAsync(keyfile);
            _key = key;
        }
    }
}
