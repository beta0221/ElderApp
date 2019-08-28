using System;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;

namespace ElderApp.ViewModels
{
    public class AccountPageVM : INotifyPropertyChanged
    {
        INavigationService _navigationService;

        private string image_url;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Image_url
        {
            get { return image_url; }
            set
            {
                image_url = value;
                OnPropertyChanged("Image_url");
            }
        }

        public string Name { get; set; }

        public string Account { get; set; }

        public string Gender { get; set; }

        public string Birthdate { get; set; }

        public string Phone { get; set; }

        public string Tel { get; set; }

        public string Address { get; set; }

        public string Id_number { get; set; }

        public string Valid { get; set; }

        public string Expriedate { get; set; }

        public ICommand Edit { get; set; }


        public AccountPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Edit = new DelegateCommand(EditRequest);
            MyAccountRequest();
        }

        private async void EditRequest()
        {
            await _navigationService.NavigateAsync("EditAccountPage");
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
                    }
                    else
                    {
                        Valid = "過期";
                    }

                    


                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Yes");
                    //await _navigationService.NavigateAsync("/NavigationPage/MyPage");
                    await _navigationService.NavigateAsync("/FirstPage?selectedTab=AccountPage");
                }
            }

        }

    }
}
