using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using ElderApp.Helpers;
using ElderApp.Interface;
using ElderApp.Models;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using SQLite;
using Xamarin.Forms;

namespace ElderApp.ViewModels
{
    public class MyPageVM : INotifyPropertyChanged
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

        public ICommand Logout { get; set; }

        public ICommand SelectImage { get; set; }

        public ICommand TakeMoney { get; set; }

        public ICommand GiveMoney { get; set; }

        public ICommand TransHistory { get; set; }

        public MyPageVM(INavigationService navigationService)
        {
            User = App.CurrentUser;
            Image_url = User.Img;
            System.Diagnostics.Debug.WriteLine(Image_url);
            Logout = new DelegateCommand(LogoutRequest);
            SelectImage = new DelegateCommand(SelectImageRequest);
            TakeMoney = new DelegateCommand(TakeMoneyRequest);
            GiveMoney = new DelegateCommand(GiveMoneyRequest);
            TransHistory= new DelegateCommand(TransHistoryRequest);
            _navigationService = navigationService;

            UpdateRequest();

        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        private async void SelectImageRequest()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await App.Current.MainPage.DisplayAlert("Not able", "Not able to pick photo", "OK");
                return;
            }

            //pick image
            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
                return;

            var stream = file.GetStream();

            byte[] byteArray = ImageConverter.StreamToByteArray(stream);
            string image_string = Convert.ToBase64String(byteArray);

            var client = new RestClient("http://61.66.218.12/api/uploadImage");
            //var client = new RestClient("http://127.0.0.1:8000/api/uploadImage");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("id", User.Id);
            request.AddParameter("name", User.Name);
            request.AddParameter("image", image_string);

            IRestResponse response = client.Execute(request);

            if (response.Content != null)
            {
                System.Diagnostics.Debug.WriteLine(response.Content);
                try
                {
                    JObject res = JObject.Parse(response.Content);
                    if (res.ContainsKey("image_name"))
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                        {
                            var _user = conn.Table<UserModel>().FirstOrDefault();
                            string image_url = $"http://61.66.218.12/images/users/{_user.Name}/{res["image_name"].ToString()}";
                            //string image_url = $"http://127.0.0.1:8000/images/users/{_user.Name}/{res["image_name"].ToString()}";
                            conn.Execute($"UPDATE UserModel SET Img = '{image_url}' WHERE Id = {_user.Id}");

                            Image_url = image_url.ToString();
                        }

                    }
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("發生錯誤", ex.ToString(), "OK");
                }



            }


        }

        private async void TakeMoneyRequest()
        {
            await _navigationService.NavigateAsync("TakeMoneyPage");
        }

        private async void GiveMoneyRequest()
        {
            //await _navigationService.NavigateAsync("ScannerPage");
            await _navigationService.NavigateAsync("GiveMoneyPage");
        }

        private async void TransHistoryRequest()
        {
            await _navigationService.NavigateAsync("TransHistoryPage");
        }


        private async void LogoutRequest()
        {

            var result = await App.Current.MainPage.DisplayAlert("Logging out", "Sure to logout", "Yes","Cancel");
            if (result == true)
            {

                //System.Diagnostics.Debug.WriteLine(App.CurrentUser.Token.ToString());
                System.Diagnostics.Debug.WriteLine("Logout");

                var client = new RestClient("http://61.66.218.12/api/auth/logout");
                //var client = new RestClient("http://127.0.0.1:8000/api/auth/logout");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", App.CurrentUser.Token.ToString());
                IRestResponse response = client.Execute(request);

                if (response.Content != null)
                {
                    try
                    {
                        JObject res = JObject.Parse(response.Content);

                        if (res["message"].ToString() == "Successfully logged out")
                        {
                            using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                            {
                                conn.Execute("DELETE FROM UserModel");

                                await _navigationService.NavigateAsync("/LoginPage");

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Yes");
                    }

                }

                
            }

        }

        private void UpdateRequest()
        {

            System.Diagnostics.Debug.WriteLine("UpdateRequest");

            var client = new RestClient("http://61.66.218.12/api/auth/me");
            //var client = new RestClient("http://127.0.0.1/api/auth/me");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token);
            IRestResponse response = client.Execute(request);

            if (response.Content != null)
            {

                JObject res = JObject.Parse(response.Content);
                if (res.ContainsKey("error"))
                {
                    AutoReLogin();
                }
                else
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                    {
                        var _user = conn.Table<UserModel>().FirstOrDefault();

                        conn.Execute($"UPDATE UserModel SET Wallet = '{Int32.Parse(res["rank"].ToString())}',Rank = '{Int32.Parse(res["wallet"].ToString())}' WHERE Id = {_user.Id}");

                        User.Rank = Int32.Parse(res["rank"].ToString());
                        User.Wallet = Int32.Parse(res["wallet"].ToString());
                    }

                        
                }

            }
        }

        private async void AutoReLogin()
        {
            System.Diagnostics.Debug.WriteLine("AutoReLogin");

            var client = new RestClient("http://61.66.218.12/api/auth/login");
            //var client = new RestClient("http://127.0.0.1:8000/api/auth/login");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("email", App.CurrentUser.Email);
            request.AddParameter("password", App.CurrentUser.Password);
            IRestResponse response = client.Execute(request);
            if (response.Content != null)
            {
                JObject res = JObject.Parse(response.Content);
                if (res.ContainsKey("access_token"))
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                    {
                        var _user = conn.Table<UserModel>().FirstOrDefault();
                        conn.Execute($"UPDATE UserModel SET Token = '{res["access_token"].ToString()}' WHERE Id = {_user.Id}");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("登入失敗", "帳號密碼錯誤", "OK");
                    await _navigationService.NavigateAsync("/LoginPage");
                }
            }
                
        }



    }
}
