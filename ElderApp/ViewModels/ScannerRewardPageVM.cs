using System;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using Xamarin.Forms;
using ZXing;

namespace ElderApp.ViewModels
{
    public class ScannerRewardPageVM: INotifyPropertyChanged
    {

        INavigationService _navigationService;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }



        private bool isScanning;
        public bool IsScanning
        {
            get { return isScanning; }
            set
            {
                isScanning = value;
                OnPropertyChanged("IsScanning");
            }
        }



        private bool isNotValidCode;
        public bool IsNotValidCode
        {
            get { return isNotValidCode; }
            set
            {
                isNotValidCode = value;
                OnPropertyChanged("IsNotValidCode");
            }
        }

        public Result Result { get; set; }

        public Command QRScanResultCommand
        {
            get
            {
                return new Command(() =>
                {


                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //do your job here - Result.Text contains QR CODE
                        //await App.Current.MainPage.DisplayAlert("Result", Result.Text, "OK");

                        IsScanning = false;
                        IsNotValidCode = false;
                        var text = Result.Text;
                        var client = new RestClient($"https://www.happybi.com.tw/api/drawEventReward/{text}");
                        
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddHeader("Accept", "application/json");
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
                                        await App.Current.MainPage.DisplayAlert("成功", "您已成功領取活動獎勵", "確定");
                                        await _navigationService.GoBackAsync();
                                    }
                                    else if(res["s"].ToString() == "0")
                                    {
                                        await App.Current.MainPage.DisplayAlert("Oops!", res["m"].ToString(), "確定");
                                        await _navigationService.GoBackAsync();
                                    }
                                }
                                else
                                {
                                    await App.Current.MainPage.DisplayAlert("發生錯誤", res.ToString(), "OK");
                                    await _navigationService.GoBackAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                await App.Current.MainPage.DisplayAlert("Decode Problem", ex.ToString(), "OK");
                            }
                        }



                    });
                });
            }
        }

        public ScannerRewardPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            IsScanning = true;
            IsNotValidCode = false;
        }

        



    }
}
