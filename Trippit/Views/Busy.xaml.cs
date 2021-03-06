using System;
using Template10.Common;
using Template10.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trippit.Views
{
    public sealed partial class Busy : UserControl
    {        
        public static event EventHandler<bool> BusyChanged;
        public static event EventHandler BusyCancelled;

        public Busy()
        {
            InitializeComponent();
        }

        public string BusyText
        {
            get { return (string)GetValue(BusyTextProperty); }
            set { SetValue(BusyTextProperty, value); }
        }
        public static readonly DependencyProperty BusyTextProperty =
            DependencyProperty.Register(nameof(BusyText), typeof(string), typeof(Busy), new PropertyMetadata("Please wait..."));

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(Busy), new PropertyMetadata(false,
                IsBusyChanged));
        private static void IsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Busy _this = d as Busy;
            if (_this == null)
            {
                return;
            }
            bool newIsBusy = (bool) e.NewValue;

            _this.RaiseBusyChanged(newIsBusy);
        }

        // hide and show busy dialog
        public static void SetBusy(bool busy, bool dismissable = false, string text = null)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;
                var view = modal.ModalContent as Busy;
                if (view == null)
                {
                    modal.ModalContent = view = new Busy();
                }
                
                if (dismissable)
                {
                    BootStrapper.BackRequested -= view.BootStrapper_BackRequested;
                    BootStrapper.BackRequested += view.BootStrapper_BackRequested;
                }

                modal.IsModal = view.IsBusy = busy;                
                view.BusyText = text;                
            });
        }

        public static void SetBusyText(string text)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;
                var view = modal.ModalContent as Busy;
                if (view == null)
                {
                    modal.ModalContent = view = new Busy();
                }

                view.BusyText = text;
            });
        }

        private void BootStrapper_BackRequested(object sender, HandledEventArgs e)
        {
            if (IsBusy)
            {
                e.Handled = true;
                SetBusy(false);
                RaiseBusyCancelled();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsBusy)
            {
                SetBusy(false);
                RaiseBusyCancelled();
            }
        }       

        private void RaiseBusyChanged(bool newIsBusy)
        {
            BusyChanged?.Invoke(this, newIsBusy);
        }        

        private void RaiseBusyCancelled()
        {
            System.Diagnostics.Debug.WriteLine("Busy cancelled early.");
            BusyCancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}

