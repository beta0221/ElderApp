using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ElderApp.Models;
using ElderApp.Services;
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

        public ICommand Signup { get; set; }

        private ApiServices service;
        public LoginPageVM(INavigationService navigationService)
        {
            service = new ApiServices();
            Login = new DelegateCommand(LoginRequest);
            Signup = new DelegateCommand(SignupRequest);
            _navigationService = navigationService;
        }


        private async void SignupRequest()
        {
            await _navigationService.NavigateAsync("/SignupPage");
        }

        private async void LoginRequest()
        {

            System.Diagnostics.Debug.WriteLine("Login");
            
            var response = await service.LoginRequest(Email,Password);

            switch (response.Item1)
            {
                case 1:
                    var res = response.Item2;
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
                            string image_url = $"{service.ApiHost}/images/users/{res["user_id"]}/{res["img"]}";
                            var newUser = new UserModel()
                            {
                                User_id = Convert.ToInt32(res["user_id"]),
                                Id_code = res["id_code"].ToString(),
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
                                await _navigationService.NavigateAsync("/FirstPage");
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("失敗", "錯誤發生", "確定");
                            }
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("登入失敗", "帳號密碼錯誤", "OK");
                    }
                    break;
                case 2:
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                    {
                        conn.Execute("DELETE FROM UserModel");
                    }
                    await App.Current.MainPage.DisplayAlert("錯誤", "系統錯誤", "確定");
                    break;
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", "伺服器無回應，網路連線錯誤。", "確定");
                    break;
                default:
                    break;
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
