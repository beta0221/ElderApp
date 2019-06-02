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

namespace ElderApp.ViewModels
{
    public class TransHistoryPageVM : IPageLifecycleAware
    {

        public ObservableCollection<Transaction> Transactions{ get; set; }


        public TransHistoryPageVM()
        {
            Transactions = new ObservableCollection<Transaction>();




            if (Device.RuntimePlatform == Device.Android)
            {
                GetTransHistory();

            }




        }

        private void GetTransHistory()
        {
            var client = new RestClient($"http://61.66.218.12/api/trans-history/{App.CurrentUser.User_id}");
            //var client = new RestClient($"http://127.0.0.1:8000/api/trans-history/{App.CurrentUser.User_id}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            IRestResponse response = client.Execute(request);

            System.Diagnostics.Debug.WriteLine(response.Content);
            if (response.Content != null)
            {
                List<Transaction> trans = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);

                Transactions.Clear();
                foreach (var tran in trans)
                {
                    Transactions.Add(tran);
                }

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
