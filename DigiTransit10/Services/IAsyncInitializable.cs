using System.Threading.Tasks;

namespace DigiTransit10.Services
{
    public interface IAsyncInitializable
    {
        /// <summary>
        /// The result of an async initialization of this instance.
        /// </summary>
        Task Initialization { get; }
    }
}