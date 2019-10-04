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
    public class CategoryPageVM : INotifyPropertyChanged
    {

        INavigationService _navigationService;

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

        //---------------------------------------------------------------------------------------------------
        private Category selectCategory { get; set; }
        public Category SelectCategory
        {
            get { return selectCategory; }
            set
            {
                selectCategory = value;
                if (selectCategory != null)
                {
                    HandledSelectItem(selectCategory);
                }
                OnPropertyChanged(nameof(SelectCategory));

            }
        }
        public async void HandledSelectItem(Category cat)
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("cat", cat);
            SelectCategory = null;
            await _navigationService.NavigateAsync("EventPage", navigationParams);
            
        }
        //---------------------------------------------------------------------------------------------------



        public CategoryPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Categories = new ObservableCollection<Category>();
            GetCategory();
        }


        private void GetCategory()
        {
            var client = new RestClient("https://www.happybi.com.tw/api/category");
            //var client = new RestClient("https://127.0.0.1:8000/api/category");

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

                    Categories.Clear();
                    foreach (var cat in _categories)
                    {
                        Categories.Add(cat);
                    }
                    Categories.Add(new Category {
                        id=0,
                        name="所有活動",
                        slug="all",
                        created_at="now"
                    });
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

