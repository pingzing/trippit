using System;
using System.Threading;
using Windows.Storage;

namespace DigiTransit10.Helpers
{
    public static class MapHelper
    {
        public static Lazy<string> MapApiToken = new Lazy<string>(() => 
            {
                StorageFile file = ExtensionMethods.TaskExtensions.RunSync(
                () => StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///maps-api-key.txt"))
                .AsTask());

                return ExtensionMethods.TaskExtensions.RunSync(
                    () => FileIO.ReadTextAsync(file)
                    .AsTask());
            }, LazyThreadSafetyMode.PublicationOnly);
    }
}
