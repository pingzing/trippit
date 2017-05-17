using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace DigiTransit10.Services
{
    public interface IDialogService
    {
        Task ShowDialog(string message);
        Task ShowDialog(string message, string title);
        Task ShowDialog(string message, string closeText, string title);
        Task<IUICommand> ShowDialog(string message, string okText, string cancelText, string title);
        Task<IUICommand> ShowDialog(MessageDialog dialog);
        Task<ContentDialogResult> ShowContentDialog<T>() where T : ContentDialog, new();
        Task<ContentDialogResult> ShowContentDialog(ContentDialog dialog);
    }

    public class DialogService : IDialogService
    {
        private static readonly SemaphoreSlim _semaphore;

        static DialogService()
        {
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task ShowDialog(string message)
        {
            var dialog = new MessageDialog(message);
            await ShowDialog(dialog);
        }

        public async Task ShowDialog(string message, string title)
        {
            var dialog = title == null ? new MessageDialog(message) : new MessageDialog(message, title);
            await ShowDialog(dialog);
        }

        public async Task ShowDialog(string message, string closeText, string title)
        {
            var dialog = title == null ? new MessageDialog(message) : new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand(closeText));
            await ShowDialog(dialog);
        }

        public async Task<IUICommand> ShowDialog(string message, string okText, string cancelText, string title)
        {
            var dialog = title == null ? new MessageDialog(message) : new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand(okText));
            dialog.Commands.Add(new UICommand(cancelText));
            dialog.CancelCommandIndex = 1;
            dialog.DefaultCommandIndex = 1;

            return await ShowDialog(dialog);
        }

        public async Task<IUICommand> ShowDialog(MessageDialog dialog)
        {
            _semaphore.Wait();

            var result = await dialog.ShowAsync();

            _semaphore.Release();

            return result;
        }

        public async Task<ContentDialogResult> ShowContentDialog<T>() where T : ContentDialog, new()
        {
            _semaphore.Wait();

            ContentDialog dialog = new T();
            ContentDialogResult result = await dialog.ShowAsync();

            _semaphore.Release();

            return result;
        }

        public async Task<ContentDialogResult> ShowContentDialog(ContentDialog dialog)
        {
            _semaphore.Wait();

            ContentDialogResult result = await dialog.ShowAsync();

            _semaphore.Release();

            return result;
        }

    }
}