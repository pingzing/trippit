using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using DigiTransit10.Helpers;

namespace DigiTransit10.Services
{
    public interface IFileService
    {
        Task<IStorageFile> GetStorageFileAsync(string path, StorageLocation location = StorageLocation.Local);
        Task<IStorageFile> GetTempFileAsync(string path, CreationCollisionOption createOptions);
    }

    public class FileService : IFileService
    {
        private readonly ApplicationData _appData;

        public FileService()
        {
            _appData = ApplicationData.Current;
        }

        public async Task<IStorageFile> GetStorageFileAsync(string path, StorageLocation location = StorageLocation.Local)
        {
            if(location == StorageLocation.Local)
            {
                return await _appData.LocalFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
            }
            else
            {
                return await _appData.RoamingFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
            }
        }

        public async Task<IStorageFile> GetTempFileAsync(string path, CreationCollisionOption createOptions)
        {
            return await _appData.LocalCacheFolder.CreateFileAsync(path, createOptions);
        }
    }

    public enum StorageLocation
    {
        Local,
        Roamed
    }
}
