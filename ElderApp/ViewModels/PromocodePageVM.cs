using System;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;

namespace ElderApp.ViewModels
{
    public class PromocodePageVM : INotifyPropertyChanged
    {
        INavigationService _navigationService;

        public ICommand Submit { get; set; }

        private string promocode;
        public string Promocode
        {
            get { return promocode; }
            set
            {
                promocode = value;
                OnPropertyChanged("Promocode");
            }
        }

        public PromocodePageVM(INavigationService navigationService)
        { 
            _navigationService = navigationService;
            Submit = new DelegateCommand(SubmitRequest);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SubmitRequest()
        {

            System.Diagnostics.Debug.WriteLine("Submit Promocode");

            if (String.IsNullOrEmpty(Promocode))
            {
                await App.Current.MainPage.DisplayAlert("錯誤", "請輸入兌換碼", "確定");
                return;
            }


            var client = new RestClient("http://www.happybi.com.tw/api/couponcode/exchange");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("promocode", Promocode);
            request.AddParameter("user_id", App.CurrentUser.User_id);
            IRestResponse response = client.Execute(request);

            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);

                    if (res.ContainsKey("s"))
                    {
                        if (res["s"].ToString() == "1")
                        {
                            await App.Current.MainPage.DisplayAlert("完成", res["m"].ToString(), "確定");
                            await _navigationService.GoBackAsync();
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("錯誤", res["m"].ToString(), "確定");
                        }
                        
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("登入失敗", "帳號密碼錯誤", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("發生錯誤", ex.ToString(), "OK");
                }
                
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("登入失敗", "伺服器連線異常", "OK");
            }


        }






    }
}
