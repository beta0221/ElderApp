using System;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using SQLite;

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

        //private string expriedate;
        //public string Expriedate
        //{
        //    get { return expriedate; }
        //    set
        //    {
        //        expriedate = value;
        //        OnPropertyChanged("Expriedate");
        //    }
        //}

        public ICommand Edit { get; set; }

        public ICommand Logout { get; set; }

        public ICommand ExtendMembership { get; set; }

        public AccountPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Edit = new DelegateCommand(EditRequest);
            Logout = new DelegateCommand(LogoutRequest);
            ExtendMembership = new DelegateCommand(ExtendRequest);

        }

        private async void EditRequest()
        {
            await _navigationService.NavigateAsync("EditAccountPage");
        }

        private async void ExtendRequest()
        {
            System.Diagnostics.Debug.WriteLine("Extend Request");

            var client = new RestClient("http://www.happybi.com.tw/api/extendMemberShip");
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

            var client = new RestClient("http://128.199.197.142/api/auth/myAccount");
            //var client = new RestClient("http://127.0.0.1:8000/api/auth/myAccount");
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
                        string image_url = $"http://128.199.197.142/images/users/{userId}/{res["img"].ToString()}";
                        Image_url = image_url.ToString();
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
                            Valid = "過期";
                            Valid_color = "#E22600";
                            Extend = true;
                        }
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

                var client = new RestClient("http://128.199.197.142/api/auth/logout");
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
                        //await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Yes");

                        using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                        {
                            conn.Execute("DELETE FROM UserModel");

                            await _navigationService.NavigateAsync("/LoginPage");

                        }
                    }

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
