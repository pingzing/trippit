using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DigiTransit10.Services
{
    public interface IFileService
    {
        Task<IStorageFile> GetStorageFileAsync(string path, StorageLocation location = StorageLocation.Local);
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
    }

    public enum StorageLocation
    {
        Local,
        Roamed
    }
}
