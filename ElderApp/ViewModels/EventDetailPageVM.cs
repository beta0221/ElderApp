using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ElderApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.AppModel;
using RestSharp;
using Xamarin.Forms;
using SQLite;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Essentials;
using ElderApp.Services;

namespace ElderApp.ViewModels
{
    public class EventDetailPageVM : INotifyPropertyChanged, INavigationAware
    {
        INavigationService _navigationService;

        private Event select_event { get; set; }
        public Event Select_event
        {
            get { return select_event; }
            set
            {
                select_event = value;
                OnPropertyChanged(nameof(Select_event));
            }
        }

        private bool par_show { get; set; }
        public bool Par_show
        {
            get { return par_show; }
            set
            {
                par_show = value;
                OnPropertyChanged(nameof(Par_show));
            }
        }

        private bool cal_show { get; set; }
        public bool Cal_show
        {
            get { return cal_show; }
            set
            {
                cal_show = value;
                OnPropertyChanged(nameof(Cal_show));
            }
        }

        public Command ButtonClick1 { get; set; }
        public Command ButtonClick2 { get; set; }

        public ICommand DrawReward { get; set; }

        public ICommand back { get; set; }

        public ICommand getPassPermit { get; set; }

        public double HeadImageHeight { get; set; }

        private ApiServices service;
        public EventDetailPageVM(Event eve, INavigationService navigationService)
        {
            service = new ApiServices();
            _navigationService = navigationService;
            back = new DelegateCommand(BackRequest);
            ButtonClick1 = new Command(ButtonClickFunction1);
            ButtonClick2 = new Command(ButtonClickFunction2);

            DrawReward = new DelegateCommand(ScanReward);
            getPassPermit = new DelegateCommand(GetPassPermit);

            //設定圖片高度比例4:3
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var density = mainDisplayInfo.Density;
            var screenWidth = mainDisplayInfo.Width / density;
            HeadImageHeight = screenWidth * 0.75;
        }
        private async void BackRequest()
        {
            await _navigationService.GoBackAsync();

        }
        private async void ButtonClickFunction1()
        {
            var eve_data = Select_event;
        
            var result = await App.Current.MainPage.DisplayAlert("參加活動確認", $"是否確認參加活動:{eve_data.title}?", "是", "否");

            var response = await service.JoinEventRequest(eve_data.slug);

            switch (response.Item1)
            {
                case 1:
                    Select_event.Participate = true;
                    Par_show = !Par_show;
                    Cal_show = !Cal_show;
                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", response.Item2, "確定");
                    break;
                default:
                    break;
            }

        }

        private async void ButtonClickFunction2()
        {
            var eve_data = Select_event;
            var result = await App.Current.MainPage.DisplayAlert("取消參加活動確認", $"是否確認取消參加活動:{eve_data.title}？", "是", "否");

            var response = await service.CancelEventRequest(eve_data.slug);

            switch (response.Item1)
            {
                case 1:
                    Select_event.Participate = false;
                    Par_show = !Par_show;
                    Cal_show = !Cal_show;
                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", response.Item2, "確定");
                    break;
                default:
                    break;
            }

        }

        private async void GetPassPermit()
        {

            


            var response = await service.isUserArrive(Select_event.slug);
            switch (response.Item1)
            {
                case 1:

                    var res = response.Item2;
                    if (res["s"].ToString() == "1")
                    {
                        //已完成報到
                        //可以顯示通行證
                        var paremeter = new NavigationParameters();
                        paremeter.Add("name", Select_event.title);
                        await _navigationService.NavigateAsync("PassPermitPage",paremeter);
                    }
                    else if (res["s"].ToString() == "2")
                    {
                        //未完成報到
                        //掃描qrcode 進行報到
                        var paremeter = new NavigationParameters();
                        paremeter.Add("scan", "arrive");
                        await _navigationService.NavigateAsync("ScannerRewardPage",paremeter);
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

        private async void ScanReward()
        {
            var paremeter = new NavigationParameters();
            paremeter.Add("scan", "reward");
            await _navigationService.NavigateAsync("ScannerRewardPage",paremeter);
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters["eve"]!=null)
            {
                Select_event = parameters["eve"] as Event;
                Par_show = !Select_event.Participate;
                Cal_show = Select_event.Participate;
            }
            
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

