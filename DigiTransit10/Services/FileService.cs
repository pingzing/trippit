using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using DigiTransit10.Helpers;
using Windows.Storage.Streams;

namespace DigiTransit10.Services
{
    public interface IFileService
    {
        Task<IStorageFile> GetStorageFileAsync(string path, StorageLocation location = StorageLocation.Local);
        Task<IStorageFolder> GetProjectFolderAsync(string path);
        Task<IStorageFile> GetTempFileAsync(string path, CreationCollisionOption createOptions);
        Task<byte[]> GetFileBytesAsync(IStorageFile file);
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

        public async Task<IStorageFolder> GetProjectFolderAsync(string path)
        {
            StorageFolder projectFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            return await projectFolder.GetFolderAsync(path);
        }

        public async Task<IStorageFile> GetTempFileAsync(string path, CreationCollisionOption createOptions)
        {
            return await _appData.LocalCacheFolder.CreateFileAsync(path, createOptions);
        }

        public async Task<byte[]> GetFileBytesAsync(IStorageFile file)
        {
            byte[] fileBytes = null;
            if (file == null)
            {
                return null;
            }
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }
    }

    public enum StorageLocation
    {
        Local,
        Roamed
    }
}
