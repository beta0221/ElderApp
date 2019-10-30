using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ElderApp.Models;
using ElderApp.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.AppModel;
using RestSharp;
using Xamarin.Forms;

namespace ElderApp.ViewModels
{
    public class TransHistoryPageVM : IPageLifecycleAware
    {

        public ObservableCollection<Transaction> Transactions{ get; set; }

        private ApiServices service;
        public TransHistoryPageVM()
        {
            service = new ApiServices();
            Transactions = new ObservableCollection<Transaction>();

            if (Device.RuntimePlatform == Device.Android)
            {
                GetTransHistory();    
            }


        }

        private async void GetTransHistory()
        {

            var response = await service.GetTransHistory();
            switch (response.Item1)
            {
                case 1:
                    var trans = response.Item2;
                    Transactions.Clear();
                    foreach (var tran in trans)
                    {
                        Transactions.Add(tran);
                    }
                    break;
                case 2:
                    await App.Current.MainPage.DisplayAlert("錯誤", "系統錯誤。", "確定");
                    break;
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", "伺服器無回應，網路連線錯誤。", "確定");
                    break;
                default:
                    break;
            }


        }


        public void OnAppearing()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                GetTransHistory();

            }
        }


        public void OnDisappearing()
        {

        }
    }
}
