using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using ElderApp.Helpers;
using ElderApp.Interface;
using ElderApp.Models;
using ElderApp.Services;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ElderApp.ViewModels
{
    public class MyPageVM : INotifyPropertyChanged , IPageLifecycleAware , IApplicationLifecycleAware
    {
        INavigationService _navigationService;


        private string image_url;
        public string Image_url
        {
            get { return image_url; }
            set
            {
                image_url = value;
                OnPropertyChanged("Image_url");
            }
        }

        private UserModel user;
        public UserModel User {
            get {return user; }
            set {
                user = value;
                OnPropertyChanged("User");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private int rank;
        public int Rank
        {
            get
            {
                return rank;
            }
            set
            {
                rank = value;
                OnPropertyChanged("Rank");
            }
        }

        private int wallet;
        public int Wallet
        {
            get
            {
                return wallet;
            }
            set
            {
                wallet = value;
                OnPropertyChanged("Wallet");
            }
        }



        public ICommand TakeMoney { get; set; }

        public ICommand GiveMoney { get; set; }

        public ICommand TransHistory { get; set; }

        public ICommand Account { get; set; }

        public ICommand Promocode { get; set; }

        

        public double SquareHeight { get; set; }

        public double SliderHeight { get; set; }

        

        public ObservableCollection<FileImageSource> Slider_images { get; set; }

        private ApiServices service;
        public MyPageVM(INavigationService navigationService)
        {
            service = new ApiServices();
            User = App.CurrentUser;
            Image_url = User.Img;
            System.Diagnostics.Debug.WriteLine(Image_url);
            
            TakeMoney = new DelegateCommand(TakeMoneyRequest);
            GiveMoney = new DelegateCommand(GiveMoneyRequest);
            TransHistory= new DelegateCommand(TransHistoryRequest);
            Promocode = new DelegateCommand(PromocodeRequest);
            

            Slider_images = new ObservableCollection<FileImageSource>();

            
            Slider_images.Add("home_min.png");
            Slider_images.Add("academy_min.png");
            Slider_images.Add("account_min.png");

            Account = new DelegateCommand(AccountRequest);

            _navigationService = navigationService;

            //if (Device.RuntimePlatform == Device.Android)
            //{
            //UpdateRequest();
            //}
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var density = mainDisplayInfo.Density;
            var screenWidth = mainDisplayInfo.Width / density;
            SquareHeight = screenWidth / 2;
            SliderHeight = screenWidth * 0.5;



        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void TakeMoneyRequest()
        {
            await _navigationService.NavigateAsync("TakeMoneyPage");
        }

        private async void GiveMoneyRequest()
        {
            await _navigationService.NavigateAsync("ScannerPage");
            //await _navigationService.NavigateAsync("GiveMoneyPage");
        }

        private async void TransHistoryRequest()
        {
            await _navigationService.NavigateAsync("TransHistoryPage");
        }

        private async void AccountRequest()
        {
            await _navigationService.NavigateAsync("AccountPage");
        }

        private async void PromocodeRequest()
        {
            await _navigationService.NavigateAsync("PromocodePage");
        }

       

        private async void UpdateRequest()
        {

            System.Diagnostics.Debug.WriteLine("UpdateRequest");

            var response = await service.MeRequest();

            switch (response.Item1)
            {
                case 1:

                    var res = response.Item2;
                    User.Rank = Int32.Parse(res["rank"].ToString());
                    User.Wallet = Int32.Parse(res["wallet"].ToString());
                    User.Name = res["name"].ToString();

                    Rank = Int32.Parse(res["rank"].ToString());
                    Wallet = Int32.Parse(res["wallet"].ToString());
                    Name = res["name"].ToString();
                    
                    break;
                case 2:
                case 3:
                    AutoReLogin();
                    break;
                default:
                    break;
            }


        }

        private async void AutoReLogin()
        {
            System.Diagnostics.Debug.WriteLine("AutoReLogin");

            var response = await service.AutoReLoginRequest();
            switch (response.Item1)
            {
                case 1:
                    var res = response.Item2;
                    if (res.ContainsKey("access_token"))
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                        {
                            var _user = conn.Table<UserModel>().FirstOrDefault();
                            conn.Execute($"UPDATE UserModel SET Token = '{res["access_token"].ToString()}' WHERE Id = {_user.Id}");
                            App.CurrentUser.Token = res["access_token"].ToString();

                            Rank = Int32.Parse(res["rank"].ToString());
                            Wallet = Int32.Parse(res["wallet"].ToString());
                        }
                    }
                    else
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                        {
                            conn.Execute("DELETE FROM UserModel");
                            await _navigationService.NavigateAsync("/LoginPage");
                        }
                    }

                    break;
                case 2:
                case 3:
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                    {
                        conn.Execute("DELETE FROM UserModel");
                        await _navigationService.NavigateAsync("/LoginPage");
                    }
                    break;
                default:
                    break;
            }

                
        }

        public void OnAppearing()
        {
            //if (Device.RuntimePlatform == Device.iOS)
            //{
            UpdateRequest();
            //}

        }

        public void OnDisappearing()
        {
            
        }

        public void OnResume()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                UpdateRequest();
            }
                
        }

        public void OnSleep()
        {
            
        }
    }
}
