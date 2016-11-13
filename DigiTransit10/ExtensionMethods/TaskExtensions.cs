using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace DigiTransit10.ExtensionMethods
{
    public static class TaskExtensions
    {
        public static void DoNotAwait(this Task task) { }

        public static void DoNotAwait<T>(this Task<T> task) { }

        public static void DoNotAwait(this IAsyncAction task) { }

        private static readonly TaskFactory _myTaskFactory = 
            new TaskFactory(CancellationToken.None,
                 TaskCreationOptions.None,
                 TaskContinuationOptions.None,
                 TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return _myTaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            _myTaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

    }
}
