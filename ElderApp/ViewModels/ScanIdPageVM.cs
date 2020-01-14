using System;
using System.ComponentModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;
using ZXing;

namespace ElderApp.ViewModels
{
    public class ScanIdPageVM: INotifyPropertyChanged
    {
        
        INavigationService _navigationService;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool isScanning;
        public bool IsScanning
        {
            get { return isScanning; }
            set
            {
                isScanning = value;
                OnPropertyChanged("IsScanning");
            }
        }

        public Command QRScanResultCommand
        {
            get
            {
                return new Command(() =>
                {


                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //do your job here - Result.Text contains QR CODE
                        //await App.Current.MainPage.DisplayAlert("Result", Result.Text, "OK");

                        IsScanning = false;

                        var paremeter = new NavigationParameters();
                        paremeter.Add("Id", Result.Text);

                        //await _navigationService.NavigateAsync("/NavigationPage/GiveMoneyPage", paremeter);

                        await _navigationService.GoBackAsync(paremeter);

                    });
                });
            }
        }

        public Result Result { get; set; }

        public ICommand Back { get; set; }

        public ScanIdPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Back = new DelegateCommand(backToSignUp);
            IsScanning = true;
        }

        private async void backToSignUp()
        {
            await _navigationService.GoBackAsync();
        }

    }
}
