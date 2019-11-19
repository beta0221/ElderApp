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
    public class ScannerRewardPageVM: INotifyPropertyChanged,INavigatedAware
    {

        INavigationService _navigationService;

        private bool reward;
        public bool Reward
        {
            get { return reward; }
            set
            {
                reward = value;
                OnPropertyChanged("Reward");
            }
        }

        private bool arrive;
        public bool Arrive
        {
            get { return arrive; }
            set
            {
                arrive = value;
                OnPropertyChanged("Arrive");
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters["scan"].ToString() == "reward")
            {
                Arrive = false;
                Reward = true;
            }
            else
            {
                Arrive = true;
                Reward = false;
            }
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

                        string[] str = Result.Text.Split(',');

                        if (str.Length == 2)
                        {

                            if (str[0] == "reward")
                            {
                                var response = await service.RrawEventReward(str[1]);

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
                            }
                            else if (str[0] == "arrive")
                            {
                                var response = await service.ArriveEvent(str[1]);
                                switch(response.Item1)
                                {
                                    case 1:

                                        var res = response.Item2;
                                        if(res["s"].ToString() == "1")
                                        {

                                            //已完成報到
                                            //可以顯示通行證

                                            var name = res["name"].ToString();
                                            var paremeter = new NavigationParameters();
                                            paremeter.Add("name", name);


                                            await _navigationService.NavigateAsync("PassPermitPage",paremeter);
                                        }
                                        else
                                        {
                                            await App.Current.MainPage.DisplayAlert("錯誤", res["m"].ToString(), "確定");
                                        }


                                        break;
                                    case 2:
                                        await App.Current.MainPage.DisplayAlert("錯誤", "系統錯誤", "確定");
                                        break;
                                    case 3:
                                        await App.Current.MainPage.DisplayAlert("錯誤", "伺服器無回應，網路連線錯誤。", "確定");
                                        break;
                                    default:
                                        break;
                                }


                            }
                            

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
