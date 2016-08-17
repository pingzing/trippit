using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace DigiTransit10.ExtensionMethods
{
    public static class TaskExtensions
    {
        public static void DoNotAwait(this Task task) { }

        public static void DoNotAwait<T>(this Task<T> task) { }

        public static void DoNotAwait(this IAsyncAction task) { }               
    }
}
