using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using ElderApp.Models;
using Newtonsoft.Json;

namespace ElderApp.ViewModels
{
    public class SignupPageVM : INotifyPropertyChanged, INavigatedAware
    {
        INavigationService _navigationService;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

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

        private string con_password;
        public string Con_password
        {
            get { return con_password; }
            set
            {
                con_password = value;
                OnPropertyChanged("Con_password");
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

        private Gender gender;
        public Gender Gender
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
                var temp = value;
                temp = temp.Substring(0,10);
                string[] words = temp.Split('/');
                birthdate = $"{words[2]}-{words[0]}-{words[1]}";
                OnPropertyChanged("Birthdate");
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

        private District district;
        public District District
        {
            get { return district; }
            set
            {
                district = value;
                OnPropertyChanged("District");
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

        private PayMethod pay_method;
        public PayMethod Pay_method
        {
            get { return pay_method; }
            set
            {
                pay_method = value;
                if (pay_method.Val == 0)
                {
                    ShowInviterInput = false;
                }
                else
                {
                    ShowInviterInput = true;
                }
                OnPropertyChanged("Pay_method");
            }
        }

        private bool showInviterInput;
        public bool ShowInviterInput
        {
            get { return showInviterInput; }
            set
            {
                showInviterInput = value;
                OnPropertyChanged("ShowInviterInput");
            }
        }

        private string inviter_id_code;
        public string Inviter_id_code
        {
            get { return inviter_id_code; }
            set
            {
                inviter_id_code = value;
                OnPropertyChanged("Inviter_id_code");
            }
        }

        private ObservableCollection<Gender> genderList;
        public ObservableCollection<Gender> GenderList
        {
            get { return genderList; }
            set
            {
                genderList = value;
                OnPropertyChanged(nameof(GenderList));
            }
        }

        private ObservableCollection<PayMethod> payMethodList;
        public ObservableCollection<PayMethod> PayMethodList
        {
            get { return payMethodList; }
            set
            {
                payMethodList = value;
                OnPropertyChanged(nameof(PayMethodList));
            }
        }

        private ObservableCollection<District> districtList;
        public ObservableCollection<District> DistrictList
        {
            get { return districtList; }
            set
            {
                districtList = value;
                OnPropertyChanged(nameof(DistrictList));
            }
        }

        public ICommand BackToLogin { get; set; }

        public ICommand Submit { get; set; }

        public ICommand Scan { get; set; }


        public SignupPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ShowInviterInput = false;
            GenderList = new ObservableCollection<Gender>();
            PayMethodList = new ObservableCollection<PayMethod>();
            DistrictList = new ObservableCollection<District>();
            BackToLogin = new DelegateCommand(BackToLoginRequest);
            Submit = new DelegateCommand(CheckInviterRequest);
            Scan = new DelegateCommand(ScanRequest);
            GetDistrict();
            initPicker();

        }

        private void initPicker()
        {
            
            GenderList.Add(new Gender{ Val = 1,Name = "男"});
            GenderList.Add(new Gender { Val = 0, Name = "女" });

            PayMethodList.Add(new PayMethod { Val = 1, Name = "推薦人代收" });
            PayMethodList.Add(new PayMethod { Val = 0, Name = "自行繳費" });

        }

        private async void BackToLoginRequest()
        {
            await _navigationService.NavigateAsync("/LoginPage");
        }

        private async void ScanRequest()
        {
            await _navigationService.NavigateAsync("ScanIdPage");
        }

        private async void SubmitRequest()
        {

            //validate here


            var client = new RestClient("https://www.happybi.com.tw/api/member/join");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("email", Email);
            request.AddParameter("password", Password);
            request.AddParameter("name", Name);
            request.AddParameter("phone", Phone);
            request.AddParameter("tel", Tel);
            request.AddParameter("gender", Gender.Val);
            request.AddParameter("birthdate", Birthdate);
            request.AddParameter("id_number", Id_number);
            request.AddParameter("district_id", District.id);
            request.AddParameter("address", Address);
            request.AddParameter("pay_method", Pay_method.Val);
            request.AddParameter("inviter_id_code", Inviter_id_code);
            request.AddParameter("app", true);

            IRestResponse response = client.Execute(request);

            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);

                    //do things here
                    if (res.ContainsKey("s"))
                    {
                        if(res["s"].ToString() == "1")
                        {
                            await App.Current.MainPage.DisplayAlert("成功", "您已成功註冊", "確定");
                            await _navigationService.NavigateAsync("/LoginPage");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("發生錯誤", res.ToString(), "OK");
                    }

                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Decode Problem", ex.ToString(), "OK");
                }


            }

                
        }



        private async void CheckInviterRequest()
        {

            //validation here



            if (Pay_method != null)
            {
                if (Pay_method.Val == 0)
                {
                    SubmitRequest();
                }
                else
                {
                    if (Inviter_id_code == null)
                    {
                        return;
                    }
                    
                    var client = new RestClient($"https://www.happybi.com.tw/api/inviterCheck?inviter_id_code={Inviter_id_code}");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddHeader("Accept", "application/json");

                    IRestResponse response = client.Execute(request);

                    if (response.Content != null)
                    {
                        try
                        {
                            JObject res = JObject.Parse(response.Content);

                            if (res["s"].ToString() == "1")
                            {

                                var result = await App.Current.MainPage.DisplayAlert("確認推薦人", $"確認推薦人姓名為：{res["inviter"].ToString()}", "是", "否");
                                if (result == true)
                                {
                                    SubmitRequest();
                                }
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("非常抱歉", "此會員編號用戶並不存在，請檢查是否輸入錯誤或向推薦人確認。", "確定");
                            }

                        }
                        catch (Exception ex)
                        {
                            await App.Current.MainPage.DisplayAlert("Decode Problem", ex.ToString(), "OK");
                        }

                    }

                }

            }
         
        }


        private async void GetDistrict()
        {
            var client = new RestClient("https://www.happybi.com.tw/api/district");

            var request = new RestRequest(Method.GET);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = client.Execute(request);
            if (response.Content != null)
            {
                try
                {
                    List<District> districtList = JsonConvert.DeserializeObject<List<District>>(response.Content);


                    foreach (var dis in districtList)
                    {

                        DistrictList.Add(dis);
                    }


                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Decode Problem", ex.ToString(), "OK");
                }
            }
        }


        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Inviter_id_code = parameters["Id"].ToString();
        }

    }//class end brecket

    


    public class NameAndVal
    {
        public int Val { get; set; }
        public string Name { get; set; }
    }

    public class Gender:NameAndVal
    {
        
    }

    public class PayMethod:NameAndVal
    {
        
    }

   

    

}
