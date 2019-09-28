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
using Xamarin.Essentials;

namespace ElderApp.ViewModels
{


    public class EventPageVM : INotifyPropertyChanged, INavigationAware
    {
        INavigationService _navigationService;

        private ObservableCollection<Event> temp_events { get; set; }




        //搜尋關鍵字
        //---------------------------------------------------------------------------------------------------
        private String searchEvent { get; set; }
        public String SearchEvent
        {
            get { return searchEvent; }
            set
            {
                searchEvent = value;
                HandledSearchItem(searchEvent);
                OnPropertyChanged(nameof(SearchEvent));

            }
        }
        public void HandledSearchItem(String text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Events = temp_events;
            }
            else
            {
                Events = temp_events;
                var test = Events.Where(c => c.title.Contains(text)).ToList();
                Events = new ObservableCollection<Event>(test);
            }

        }


        //選擇的地區
        //---------------------------------------------------------------------------------------------------
        private District selectDistrict;
        public District SelectDistrict
        {
            get {
                if (selectDistrict == null)
                {
                    return new District { id=0,name="所有地區"};
                }
                return selectDistrict;
            }
            set
            {
                selectDistrict = value;
                OnPropertyChanged(nameof(SelectDistrict));
                GetEvents();
            }
        }



        //選擇的類別
        //---------------------------------------------------------------------------------------------------
        private Category selectCategory;
        public Category SelectCategory
        {
            get {
                if (selectCategory == null)
                {
                    return new Category { id = 0, name = "所有類別" };
                }
                return selectCategory;
            }
            set
            {
                selectCategory = value;
                OnPropertyChanged(nameof(SelectCategory));
                GetEvents();
            }
        }
        

        //選擇的活動
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

        //類別
        //---------------------------------------------------------------------------------------------------
        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }


        //地區
        //---------------------------------------------------------------------------------------------------
        private ObservableCollection<District> districts;
        public ObservableCollection<District> Districts
        {
            get { return districts; }
            set
            {
                districts = value;
                OnPropertyChanged(nameof(Districts));
            }
        }


        //活動
        //---------------------------------------------------------------------------------------------------
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

        public double SliderHeight { get; set; }

        public ICommand My_events { get; set; }     //我的活動

        private Dictionary<int,string> DistrictDictionary { get; set; }

        private Dictionary<int, string> CategoryDictionary { get; set; }

        public EventPageVM(INavigationService navigationService)
        {

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var density = mainDisplayInfo.Density;
            var screenWidth = mainDisplayInfo.Width / density;
            SliderHeight = screenWidth * 0.5;
            My_events = new DelegateCommand(My_eventsRequest);


            _navigationService = navigationService;
            Events = new ObservableCollection<Event>();
            My_events_id = new List<int>();
            temp_events = new ObservableCollection<Event>();
            Categories = new ObservableCollection<Category>();
            Districts = new ObservableCollection<District>();

            GetCategory();
            GetDistrict();
            
            GetUserEvent();
            GetEvents();
        }


        private async void My_eventsRequest()                      //我的活動
        {
            await _navigationService.NavigateAsync("MyEventPage");
        }

        private async void GetDistrict()
        {
            var client = new RestClient("http://128.199.197.142/api/district");

            var request = new RestRequest(Method.GET);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = client.Execute(request);
            if (response.Content != null)
            {
                try
                {
                    List<District> districtList = JsonConvert.DeserializeObject<List<District>>(response.Content);

                    var districtDic = new Dictionary<int,string>();

                    Districts.Add(new District
                    {
                        id=0,
                        name="所有地區",
                    });
                    foreach (var dis in districtList)
                    {

                        districtDic.Add(dis.id, dis.name);
                        Districts.Add(dis);
                    }

                    DistrictDictionary = districtDic;

                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Decode Problem", ex.ToString(), "OK");
                }
            }

        }

        private void GetCategory()
        {
            var client = new RestClient("http://128.199.197.142/api/category");

            var request = new RestRequest(Method.GET);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = client.Execute(request);
            //System.Diagnostics.Debug.WriteLine(response.Content);

            if (response.Content != null)
            {
                if (response.Content != null)
                {
                    List<Category> _categories = JsonConvert.DeserializeObject<List<Category>>(response.Content);

                    var categoryDic = new Dictionary<int, string>();

                    Categories.Clear();
                    Categories.Add(new Category
                    {
                        id = 0,
                        name = "所有類別",
                        slug = "all",
                        created_at = "now"
                    });
                    foreach (var cat in _categories)
                    {
                        Categories.Add(cat);
                        categoryDic.Add(cat.id, cat.name);
                    }

                    CategoryDictionary = categoryDic;

                }
            }
        }



        private void GetEvents()
        {

            
            var client = new RestClient($"http://128.199.197.142/api/event?category={SelectCategory.id}&district={SelectDistrict.id}");
            
            var request = new RestRequest(Method.GET);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = client.Execute(request);
            if (response.Content != null)
            {

                if (response.Content != null)
                {
                    List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(response.Content);

                    Events.Clear();
                    foreach (var eve in _events)
                    {
                        //int ind = My_events_id.IndexOf(eve.id);
                        //if (ind < 0)
                        //{
                        //    eve.Participate = false;
                        //}
                        //else
                        //{
                        //    eve.Participate = true;
                        //}
                        //var a = DistrictDictionary[eve.district_id];
                        eve.district_name = DistrictDictionary[eve.district_id];
                        eve.category_name = CategoryDictionary[eve.category_id];

                        Events.Add(eve);
                    }
                    temp_events = Events;
                }
            }
            
            
        }

        private void GetUserEvent()
        {
            System.Diagnostics.Debug.WriteLine("GetUserEvent");

            var client = new RestClient("http://128.199.197.142/api/myevent");
            //var client = new RestClient("http://127.0.0.1:8000/api/myevent");

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("id", App.CurrentUser.User_id);
            request.AddParameter("token", App.CurrentUser.Token.ToString());

            IRestResponse response = client.Execute(request);

            if (response.Content != null)
            {
                if (response.Content != null)
                {
                    System.Diagnostics.Debug.WriteLine(response.Content);

                    List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(response.Content);     

                    My_events_id.Clear();
                    foreach (var eve in _events)
                    {
                        My_events_id.Add(eve.id);
                    }

                }
            }
        }



        


        public void OnNavigatedTo(INavigationParameters parameters)
        {
          
            //GetCategory();
            //SelectCategory = (Categories.Where(c => c.slug == "all").ToList())[0];
            //This_category = SelectCategory;
            

            //GetUserEvent();
            //GetEvents();
            
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            //GetEvents();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            //GetEvents();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }



    
}

