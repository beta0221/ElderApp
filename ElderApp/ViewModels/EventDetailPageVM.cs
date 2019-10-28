﻿using System;
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

        public double HeadImageHeight { get; set; }

        public EventDetailPageVM(Event eve, INavigationService navigationService)
        {
            _navigationService = navigationService;
            back = new DelegateCommand(BackRequest);
            ButtonClick1 = new Command(ButtonClickFunction1);
            ButtonClick2 = new Command(ButtonClickFunction2);

            DrawReward = new DelegateCommand(ScanReward);

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

            var service = new ApiServices();
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



            var service = new ApiServices();
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


        private async void ScanReward()
        {
            await _navigationService.NavigateAsync("ScannerRewardPage");
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

