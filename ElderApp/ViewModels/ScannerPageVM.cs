using System;
using System.ComponentModel;
using System.Windows.Input;

using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;
using ZXing;

namespace ElderApp.ViewModels
{
    public class ScannerPageVM : INotifyPropertyChanged
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

        

        private bool isNotValidCode;
        public bool IsNotValidCode
        {
            get { return isNotValidCode; }
            set
            {
                isNotValidCode = value;
                OnPropertyChanged("IsNotValidCode");
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
                        IsNotValidCode = false;

                        string[] a = Result.Text.Split(',');

                        if(a.Length == 3)
                        {
                            var paremeter = new NavigationParameters();
                            paremeter.Add("User_id", a[0]);
                            paremeter.Add("User_name", a[1]);
                            paremeter.Add("User_email", a[2]);

                            await _navigationService.NavigateAsync("/NavigationPage/GiveMoneyPage", paremeter);
                        }
                        else
                        {
                            IsScanning = false;
                            IsNotValidCode = true;
                        }

                        

                        
                    });
                });
            }
        }

        public Result Result { get; set; }

        public ScannerPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            IsScanning = true;
            IsNotValidCode = false;
        }

        
    }
}
