﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using ElderApp.Helpers;
using ElderApp.Models;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using SQLite;
using Xamarin.Forms;

namespace ElderApp.ViewModels
{
    public class AccountPageVM : INotifyPropertyChanged, IPageLifecycleAware
    {
        INavigationService _navigationService;

        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

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

        private string account;
        public string Account
        {
            get { return account; }
            set
            {
                account = value;
                OnPropertyChanged("Account");
            }
        }

        private string gender;
        public string Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                OnPropertyChanged("Gender");
            }
        }

        private string birthdate;
        public string Birthdate
        {
            get { return birthdate; }
            set
            {
                birthdate = value;
                OnPropertyChanged("Birthdate");
            }
        }

        private string phone;
        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                OnPropertyChanged("Phone");
            }
        }

        private string tel;
        public string Tel
        {
            get { return tel; }
            set
            {
                tel = value;
                OnPropertyChanged("Tel");
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        private string id_number;
        public string Id_number
        {
            get { return id_number; }
            set
            {
                id_number = value;
                OnPropertyChanged("Id_number");
            }
        }

        private string valid;
        public string Valid
        {
            get { return valid; }
            set
            {
                valid = value;
                OnPropertyChanged("Valid");
            }
        }

        private string valid_color;
        public string Valid_color
        {
            get { return valid_color; }
            set
            {
                valid_color = value;
                OnPropertyChanged("Valid_color");
            }
        }

        private bool extend;
        public bool Extend
        {
            get { return extend; }
            set
            {
                extend = value;
                OnPropertyChanged("Extend");
            }
        }

        private string id_code;
        public string Id_code
        {
            get { return id_code; }
            set
            {
                id_code = value;
                OnPropertyChanged("Id_code");
            }
        }



        public ICommand Edit { get; set; }

        public ICommand Logout { get; set; }

        public ICommand ExtendMembership { get; set; }

        public ICommand SelectImage { get; set; }

        public AccountPageVM(INavigationService navigationService)
        {
            
            _navigationService = navigationService;
            Edit = new DelegateCommand(EditRequest);
            Logout = new DelegateCommand(LogoutRequest);
            ExtendMembership = new DelegateCommand(ExtendRequest);
            SelectImage = new DelegateCommand(SelectImageRequest);

        }


        private async void EditRequest()
        {
            await _navigationService.NavigateAsync("EditAccountPage");
        }

        private async void ExtendRequest()
        {
            System.Diagnostics.Debug.WriteLine("Extend Request");

            var client = new RestClient("https://www.happybi.com.tw/api/extendMemberShip");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token.ToString());
            request.AddParameter("user_id", App.CurrentUser.User_id);
            IRestResponse response = client.Execute(request);

            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);
                    if (res.ContainsKey("s"))
                    {

                        await App.Current.MainPage.DisplayAlert("訊息", res["m"].ToString(), "確定");
                        //if(res["s"].ToString() == "1")
                        //{
                        //    MyAccountRequest();
                        //}
                    }

                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Yes");
                    await _navigationService.NavigateAsync("/FirstPage?selectedTab=MyPage");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("錯誤", "伺服器無回應或無網路服務", "確定");
            }



        }

        private async void MyAccountRequest()
        {
            System.Diagnostics.Debug.WriteLine("MyAccount");

            var client = new RestClient("https://www.happybi.com.tw/api/auth/myAccount");
            //var client = new RestClient("https://127.0.0.1:8000/api/auth/myAccount");
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
                    if (res.ContainsKey("img"))
                    {
                        var userId = App.CurrentUser.User_id;
                        string image_url = "";
                        System.Diagnostics.Debug.WriteLine(res["img"].ToString());
                        if (String.IsNullOrEmpty(res["img"].ToString()))
                        {

                            image_url = "user_default.png";
                        }
                        else
                        {
                            image_url = $"https://www.happybi.com.tw/images/users/{userId}/{res["img"].ToString()}";
                        }
                        
                        Image_url = image_url;
                        Name = res["name"].ToString();
                        Account = res["email"].ToString();
                        if (res["gender"].ToString() == "1")
                        {
                            Gender = "男";
                        }
                        else
                        {
                            Gender = "女";
                        }

                        Birthdate = res["birthdate"].ToString();
                        Phone = res["phone"].ToString();
                        Tel = res["tel"].ToString();
                        Address = res["address"].ToString();
                        Id_number = res["id_number"].ToString();

                        if (res["valid"].ToString() == "1")
                        {
                            Valid = "有效";
                            Valid_color = "Green";
                            Extend = false;
                        }
                        else
                        {
                            Valid = "無效";
                            Valid_color = "#E22600";
                            Extend = true;
                        }

                        Id_code = res["id_code"].ToString();

                    }
                    else
                    {
                        //update token here
                    }

                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Yes");
                    //await _navigationService.NavigateAsync("/NavigationPage/MyPage");
                    await _navigationService.NavigateAsync("/FirstPage?selectedTab=MyPage");
                }
            }

        }

        private async void LogoutRequest()
        {

            var result = await App.Current.MainPage.DisplayAlert("Logging out", "Sure to logout", "Yes", "Cancel");
            if (result == true)
            {

                //System.Diagnostics.Debug.WriteLine(App.CurrentUser.Token.ToString());
                System.Diagnostics.Debug.WriteLine("Logout");

                var client = new RestClient("https://www.happybi.com.tw/api/auth/logout");
                
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

                        using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                        {
                            conn.Execute("DELETE FROM UserModel");
                            
                            await _navigationService.NavigateAsync("/LoginPage");
                            
                        }


                    }

                }


            }

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

            var client = new RestClient("https://www.happybi.com.tw/api/auth/uploadImage");
            
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token);
            request.AddParameter("id", App.CurrentUser.User_id.ToString());
            request.AddParameter("name", App.CurrentUser.Name);
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
                            string image_url = $"https://www.happybi.com.tw/images/users/{_user.User_id}/{res["image_name"].ToString()}";
                            //string image_url = $"http://127.0.0.1:8000/images/users/{_user.User_id}/{res["image_name"].ToString()}";
                            conn.Execute($"UPDATE UserModel SET Img = '{image_url}' WHERE Id = {_user.Id}");

                            Image_url = image_url.ToString();
                            App.CurrentUser.Img = image_url.ToString();
                        }

                    }
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("發生錯誤", ex.ToString(), "OK");
                }



            }


        }

        public void OnAppearing()
        {
            MyAccountRequest();
        }

        public void OnDisappearing()
        {
            
        }
    }
}
