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


    public class EventPageVM : INotifyPropertyChanged
    {
        INavigationService _navigationService;

        private ObservableCollection<Event> temp_events { get; set; }

        private ObservableCollection<Event> events ;
        public ObservableCollection<Event> Events
        {
            get { return events; }
            set
            {
                events = value;
                OnPropertyChanged(nameof(Events));
            }
        }
        private List<int> my_events_id;
        public List<int> My_events_id
        {
            get { return my_events_id; }
            set
            {
                my_events_id = value;
                OnPropertyChanged("My_events_id");
            }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        //---------------------------------------------------------------------------------------------------
        private String searchEvent { get; set; }
        public String SearchEvent
        {
            get { return searchEvent; }
            set
            {
                searchEvent = value;
                //if(string.IsNullOrWhiteSpace(searchEvent))
                //{
                //    Events = temp_events;
                //}
                //else
                //{
                //    HandledSearchItem(searchEvent);
                //}
                HandledSearchItem(searchEvent);
                OnPropertyChanged(nameof(SearchEvent));

            }
        }
        public void HandledSearchItem(String text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Events = temp_events;
            }else
            {
                Events = temp_events;
                var test = Events.Where(c => c.title.StartsWith(text)).ToList();
                Events = new ObservableCollection<Event>(test);
            }
            
            //System.Diagnostics.Debug.WriteLine(text);
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
        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    GetUserEvent();
                    GetEvents();

                    IsRefreshing = false;
                });
            }
        }

        public Command ButtonClick { get; set; }
        //public ICommand ButtonClick
        //{
        //    get
        //    {
        //        return new Command<int>((x) => ButtonClickFunction(x));
        //    }
        //}
        //public void ButtonClickFunction(int x)
        //{
        //    System.Diagnostics.Debug.WriteLine(x);
        //}                                                   //按鈕command



        public EventPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Events = new ObservableCollection<Event>();
            My_events_id = new List<int>();
            temp_events = new ObservableCollection<Event>();
            ButtonClick = new Command(ButtonClickFunction);


            GetUserEvent();
            GetEvents();
        }

        private async void ButtonClickFunction(object sender)
        {
            var eve_data = sender as Event;
            
                
            //System.Diagnostics.Debug.WriteLine(eve_data.id);

            RestClient client;
            bool result;
            if (eve_data.Participate == true)
            {
                result = await App.Current.MainPage.DisplayAlert("取消參加活動確認", $"是否確認取消參加活動:{eve_data.title}？", "是", "否");
                
                //var client = new RestClient($"http://61.66.218.12/api/cancelevent/{eve_data.slug}");
                client = new RestClient($"http://127.0.0.1:8000/api/cancelevent/{eve_data.slug}");
            }else
            {
                result = await App.Current.MainPage.DisplayAlert("參加活動確認", $"是否確認參加活動:{eve_data.title}?", "是", "否");
               
                //var client = new RestClient($"http://61.66.218.12/api/joinevent/{eve_data.slug}");
                client = new RestClient($"http://127.0.0.1:8000/api/joinevent/{eve_data.slug}");
            }

            
            if(result==true)
            {
                var request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Accept", "application/json");
                request.AddParameter("id", App.CurrentUser.User_id);
                request.AddParameter("token", App.CurrentUser.Token.ToString());

                IRestResponse response = client.Execute(request);
                if(response.Content!=null)
                {
                    GetUserEvent();
                    GetEvents();
                    HandledSearchItem(SearchEvent);
                }
            }
            

            

        }
        


        private void GetEvents()
        {
            System.Diagnostics.Debug.WriteLine("GetEvents");

            //var client = new RestClient("http://61.66.218.12/api/event");
            var client = new RestClient("http://127.0.0.1:8000/api/event");

            var request = new RestRequest(Method.GET);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = client.Execute(request);
            System.Diagnostics.Debug.WriteLine(response.Content);

            if (response.Content != null)
            {

                //JObject res = JObject.Parse(response.Content);              //錯誤
                if (response.Content != null)
                {
                    List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(response.Content);

                    Events.Clear();                         
                    foreach (var eve in _events)
                    {
                        int ind = My_events_id.IndexOf(eve.id);
                        if (ind < 0)
                        {
                            eve.Participate = false;
                        }
                        else
                        {
                            eve.Participate = true;
                        }
                        Events.Add(eve);
                    }
                    temp_events = Events;
                }
            }
        }

        private void GetUserEvent()
        {
            System.Diagnostics.Debug.WriteLine("GetUserEvent");

            //var client = new RestClient("http://61.66.218.12/api/myevent");
            var client = new RestClient("http://127.0.0.1:8000/api/myevent");

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("id", App.CurrentUser.User_id);
            request.AddParameter("token", App.CurrentUser.Token.ToString());

            IRestResponse response = client.Execute(request);

            if (response.Content != null)
            {

                //JObject res = JObject.Parse(response.Content);

                //foreach(var re in res)
                //{
                //    my_events_id.Add(res["id"].ToString());
                //}
                if (response.Content != null)
                {
                    System.Diagnostics.Debug.WriteLine(response.Content);

                    List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(response.Content);     //錯誤

                    My_events_id.Clear();
                    foreach (var eve in _events)
                    {
                        My_events_id.Add(eve.id);
                    }

                }
            }
        }

       

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }



    
}

