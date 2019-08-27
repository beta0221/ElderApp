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
using System.Linq;

namespace ElderApp.ViewModels
{
    public class MyEventPageVM : INotifyPropertyChanged, INavigationAware

    {

        INavigationService _navigationService;

        private ObservableCollection<Event> temp_my_events { get; set; }

        private ObservableCollection<Event> my_events;
        public ObservableCollection<Event> My_Events
        {
            get { return my_events; }
            set
            {
                my_events = value;
                OnPropertyChanged(nameof(My_Events));
            }
        }

        //---------------------------------------------------------------------------------------------------
        private Event selectEvent { get; set; }
        public Event SelectEvent
        {
            get { return selectEvent; }
            set
            {
                selectEvent = value;
                if (selectEvent != null)
                {
                    HandledSelectItem(selectEvent);
                }
                OnPropertyChanged(nameof(SelectEvent));

            }
        }
        public async void HandledSelectItem(Event eve)
        {

            SelectEvent = null;
            var navigationParams = new NavigationParameters();
            navigationParams.Add("eve", eve);
            //System.Diagnostics.Debug.WriteLine(eve.id);
            await _navigationService.NavigateAsync("EventDetailPage", navigationParams);
            //await _navigationService.NavigateAsync("EventDetailPage");

        }
        //---------------------------------------------------------------------------------------------------
        public Command ButtonClick { get; set; }



        public MyEventPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            My_Events = new ObservableCollection<Event>();
            ButtonClick = new Command(ButtonClickFunction);
            
        }



        private async void ButtonClickFunction(object sender)
        {
            var eve_data = sender as Event;

            RestClient client;
           
            var result = await App.Current.MainPage.DisplayAlert("取消參加活動確認", $"是否確認取消參加活動:{eve_data.title}？", "是", "否");

            client = new RestClient($"http://128.199.197.142/api/cancelevent/{eve_data.slug}");
            //client = new RestClient($"http://127.0.0.1:8000/api/cancelevent/{eve_data.slug}");



            if (result == true)
            {
                var request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Accept", "application/json");
                request.AddParameter("id", App.CurrentUser.User_id);
                request.AddParameter("token", App.CurrentUser.Token.ToString());

                IRestResponse response = client.Execute(request);
                if (response.Content != null)
                {
                    GetMyEvents();

                }
            }

        }

        private void GetMyEvents()
        {
            System.Diagnostics.Debug.WriteLine("GetMyEvents");

            var client = new RestClient("http://128.199.197.142/api/myevent");
            //var client = new RestClient("http://127.0.0.1:8000/api/myevent");

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("id", App.CurrentUser.User_id);
            request.AddParameter("token", App.CurrentUser.Token.ToString());

            IRestResponse response = client.Execute(request);

            System.Diagnostics.Debug.WriteLine(response.Content);

            if (response.Content != null)
            {

                //JObject res = JObject.Parse(response.Content);              //錯誤
                if (response.Content != null)
                {
                    List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(response.Content);

                    My_Events.Clear();
                    foreach (var eve in _events)
                    {
                        
                        eve.Participate = true;
                        
                        My_Events.Add(eve);
                    }
                    temp_my_events = My_Events;
                }
            }
        }


        public void OnNavigatedTo(INavigationParameters parameters)
        {
            GetMyEvents();
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

