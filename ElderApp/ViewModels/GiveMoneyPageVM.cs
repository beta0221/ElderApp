using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using ElderApp.Services;
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

        private string _event;
        public string Event
        {
            get { return _event; }
            set
            {
                _event = value;
                OnPropertyChanged("Event");
            }
        }

        public ICommand SubmitTransaction { get; set; }

        public ICommand CancelTransaction { get; set; }

        private ApiServices service;
        public GiveMoneyPageVM(INavigationService navigationService)
        {
            service = new ApiServices();
            _navigationService = navigationService;
            SubmitTransaction = new DelegateCommand(SubmitTransactionRequest);
            CancelTransaction = new DelegateCommand(CancelTransactionRequest);

        }


        private async void SubmitTransactionRequest()
        {

            if (Amount < 1)
            {
                await App.Current.MainPage.DisplayAlert("錯誤", "請確認支付金額", "確定");
                return;
            }

            var response = await service.TransactionRequest(User_id,User_email,Amount,Event);
            switch (response.Item1)
            {
                case 1:
                    if (response.Item2 == "success")
                    {
                        await App.Current.MainPage.DisplayAlert("支付成功", "回首頁", "確定");
                        await _navigationService.NavigateAsync("/FirstPage");
                    }
                    else if (response.Item2 == "insufficient")
                    {
                        await App.Current.MainPage.DisplayAlert("失敗", "剩餘樂幣不足", "確定");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("失敗", response.Item2, "確定");
                        await _navigationService.NavigateAsync("/FirstPage");
                    }
                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("失敗", response.Item2, "確定");
                    await _navigationService.NavigateAsync("/FirstPage");
                    break;
                default:
                    break;
            }

        }

        private async void CancelTransactionRequest()
        {
            await _navigationService.NavigateAsync("/FirstPage");
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
            User_name = UnicodeToString(parameters["User_name"].ToString());
        }

        private string UnicodeToString(string srcText)
        {
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;

            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        }
    }
}
