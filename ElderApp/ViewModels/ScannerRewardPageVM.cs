using System;
using System.ComponentModel;
using ElderApp.Services;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using Xamarin.Forms;
using ZXing;

namespace ElderApp.ViewModels
{
    public class ScannerRewardPageVM: INotifyPropertyChanged
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

        public Result Result { get; set; }

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
                        var slug = Result.Text;

                        var response = await service.RrawEventReward(slug);

                        switch (response.Item1)
                        {
                            case 1:
                                await App.Current.MainPage.DisplayAlert("成功", "您已成功領取活動獎勵", "確定");
                                await _navigationService.GoBackAsync();
                                break;
                            case 2:
                            case 3:
                                await App.Current.MainPage.DisplayAlert("錯誤", response.Item2, "確定");
                                await _navigationService.GoBackAsync();
                                break;
                            default:
                                break;
                        }


                    });
                });
            }
        }

        private ApiServices service;
        public ScannerRewardPageVM(INavigationService navigationService)
        {
            service = new ApiServices();
            _navigationService = navigationService;
            IsScanning = true;
            IsNotValidCode = false;
        }

        



    }
}
