using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;
using ZXing;

namespace ElderApp.ViewModels
{
    public class ScannerPageVM
    {

        INavigationService _navigationService;

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


                        string[] a = Result.Text.Split(',');


                        var paremeter = new NavigationParameters();
                        paremeter.Add("User_id",a[0]);
                        paremeter.Add("User_name", a[1]);
                        paremeter.Add("User_email", a[2]);

                        await _navigationService.NavigateAsync("GiveMoneyPage",paremeter);
                    });
                });
            }
        }

        public Result Result { get; set; }

        public ScannerPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }


    }
}
