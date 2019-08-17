using System;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;

namespace ElderApp.ViewModels
{
    public class GiveMoneyPageVM : INotifyPropertyChanged, INavigatedAware
    {
        INavigationService _navigationService;

        private int user_id;
        public int User_id 
        {
            get { return user_id; } 
            set { 
                user_id = value;
                OnPropertyChanged("User_id");
            }
        }

        private string user_name;
        public string User_name 
        {
            get { return user_name; }
            set
            {
                user_name = value;
                OnPropertyChanged("User_name");
            }
        }

        private string user_email;
        public string User_email 
        {
            get { return user_email; }
            set
            {
                user_email = value;
                OnPropertyChanged("User_email");
            }
        }

        private int amount;
        public int Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnPropertyChanged("Amount");
            }
        }

        public ICommand SubmitTransaction { get; set; }

        public GiveMoneyPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SubmitTransaction = new DelegateCommand(SubmitTransactionRequest);


            User_email = "user@user.com";
            User_id = 1;
            User_name = "user";
        }


        private async void SubmitTransactionRequest()
        {
            var client = new RestClient("http://128.199.197.142/api/transaction");
            //var client = new RestClient("http://127.0.0.1:8000/api/transaction");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("give_id", App.CurrentUser.User_id);
            request.AddParameter("give_email", App.CurrentUser.Email);
            request.AddParameter("take_id", User_id);
            request.AddParameter("take_email", User_email);
            request.AddParameter("amount", Amount);
            request.AddParameter("event", "test 123");
            IRestResponse response = client.Execute(request);

            if (response.Content.ToString() == "success")
            {
                await App.Current.MainPage.DisplayAlert("支付成功", "回首頁", "OK");
                //await _navigationService.GoBackAsync();
                await _navigationService.NavigateAsync("/NavigationPage/MyPage");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("失敗", "不明原因", "OK");
                //await _navigationService.GoBackAsync();
                await _navigationService.NavigateAsync("/NavigationPage/MyPage");
            }


            //System.Diagnostics.Debug.WriteLine(response.Content.ToString());

        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            User_email = parameters["User_email"].ToString();
            User_id = Int32.Parse(parameters["User_id"].ToString());
            User_name = parameters["User_name"].ToString();
        }
    }
}
