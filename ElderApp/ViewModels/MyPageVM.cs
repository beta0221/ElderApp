using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using ElderApp.Helpers;
using ElderApp.Interface;
using ElderApp.Models;
using Foundation;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using SQLite;
using UIKit;
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
            Logout = new DelegateCommand(LogutRequest);
            SelectImage = new DelegateCommand(SelectImageRequest);
            TakeMoney = new DelegateCommand(TakeMoneyRequest);
            GiveMoney = new DelegateCommand(GiveMoneyRequest);
            TransHistory= new DelegateCommand(TransHistoryRequest);
            _navigationService = navigationService;


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

            //var client = new RestClient("http://61.66.218.12/api/uploadImage");
            var client = new RestClient("http://127.0.0.1:8000/api/uploadImage");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("id", User.Id);
            request.AddParameter("name", User.Name);
            request.AddParameter("image", image_string);

            IRestResponse response = client.Execute(request);



            if (response.Content != null)
            {
                JObject res = JObject.Parse(response.Content);

                if (res.ContainsKey("image_name"))
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                    {
                        var _user = conn.Table<UserModel>().FirstOrDefault();
                        //string image_url = $"http://61.66.218.12/images/users/{_user.Name}/{res["image_name"]}";
                        string image_url = $"http://127.0.0.1:8000/images/users/{_user.Name}/{res["image_name"]}";
                        conn.Execute($"UPDATE UserModel SET Img = '{image_url}' WHERE Id = {_user.Id}");

                        Image_url = image_url;
                    }


                }
            }


        }

        private async void TakeMoneyRequest()
        {
            await _navigationService.NavigateAsync("TakeMoneyPage");
        }

        private async void GiveMoneyRequest()
        {

            //var paremeter = new NavigationParameters();
            //paremeter.Add("User_id","2");
            //paremeter.Add("User_name", "test");
            //paremeter.Add("User_email", "test@test.com");

            //await _navigationService.NavigateAsync("GiveMoneyPage",paremeter);

            await _navigationService.NavigateAsync("ScannerPage");
        }

        private async void TransHistoryRequest()
        {
            await _navigationService.NavigateAsync("TransHistoryPage");
        }


        private async void LogutRequest()
        {

            var result = await App.Current.MainPage.DisplayAlert("Logging out", "Sure to logout", "Yes","Cancel");
            if (result == true)
            {

                //System.Diagnostics.Debug.WriteLine(App.CurrentUser.Token.ToString());


                //var client = new RestClient("http://61.66.218.12/api/auth/logout");
                //var client = new RestClient("http://127.0.0.1:8000/api/auth/logout");
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                //request.AddHeader("Accept", "application/json");
                //request.AddParameter("token", App.CurrentUser.Token.ToString());
                //IRestResponse response = client.Execute(request);

                //JObject res = JObject.Parse(response.Content);

                //if (res["message"].ToString() == "Successfully logged out")
                //{
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                    {
                        conn.Execute("DELETE FROM UserModel");

                        await _navigationService.NavigateAsync("/LoginPage");

                    }
                //}

                
            }


        }

    }
}
