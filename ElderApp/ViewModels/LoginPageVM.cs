using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ElderApp.Models;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using SQLite;

namespace ElderApp.ViewModels
{
    public class LoginPageVM :INotifyPropertyChanged
    {

        INavigationService _navigationService;

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }



        public ICommand Login { get; set; }

        public LoginPageVM(INavigationService navigationService)
        {
            Login = new DelegateCommand(LoginRequest);
            _navigationService = navigationService;
        }


        private async void LoginRequest()
        {

            System.Diagnostics.Debug.WriteLine("Login");

            var client = new RestClient("http://128.199.197.142/api/auth/login");
            //var client = new RestClient("http://127.0.0.1:8000/api/auth/login");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("email", Email);
            request.AddParameter("password", Password);
            IRestResponse response = client.Execute(request);

            if (response.Content!=null)
            {
                JObject res = JObject.Parse(response.Content);

                if (res.ContainsKey("access_token"))
                {

                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                    {
                        try
                        {
                            conn.Execute("DELETE FROM UserModel");
                        }
                        catch (Exception ex)
                        {
                            var properties = new Dictionary<string, string>
                            {
                                {"error","Delete from UserModel fail"}
                            };
                            Crashes.TrackError(ex, properties);
                        }


                        conn.CreateTable<UserModel>();
                        string image_url = $"http://128.199.197.142/images/users/{res["user_id"]}/{res["img"]}";
                        //string image_url = $"http://127.0.0.1:8000/images/users/{res["user_id"]}/{res["img"]}";

                        var newUser = new UserModel()
                        {
                            User_id = Convert.ToInt32(res["user_id"]),
                            Email = res["email"].ToString(),
                            Name = res["name"].ToString(),
                            Password = Password,
                            Wallet = Convert.ToInt32(res["wallet"]),
                            Rank = Convert.ToInt32(res["rank"]),
                            Img = image_url,
                            Token = res["access_token"].ToString(),

                        };

                        int inserted = conn.Insert(newUser);

                        if (inserted >= 1)
                        {
                            App.CurrentUser = newUser;
                            await _navigationService.NavigateAsync("NavigationPage/MyPage");
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Failure", "An error occured", "OK");
                        }

                    }

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("登入失敗", "帳號密碼錯誤", "OK");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("登入失敗", "伺服器連線異常", "OK");
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
