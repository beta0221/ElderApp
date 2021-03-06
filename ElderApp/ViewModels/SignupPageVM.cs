﻿using System;
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
using ElderApp.Services;

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
            get {
                return gender;
            }
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

        private ApiServices service;
        public SignupPageVM(INavigationService navigationService)
        {
            service = new ApiServices();
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

            
            var response = await service.SignUpRequest(Email,Password,Name,Phone,Tel,Gender.Val,Birthdate,Id_number,District.id,Address,Pay_method.Val,Inviter_id_code);
            switch (response.Item1)
            {
                case 1:
                    await App.Current.MainPage.DisplayAlert("成功", response.Item2, "確定");
                    await _navigationService.NavigateAsync("/LoginPage");
                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", response.Item2, "確定");
                    break;
                default:
                    break;
            }
                
        }



        private async void CheckInviterRequest()
        {
            if (!isValid())
            {
                await App.Current.MainPage.DisplayAlert("提醒", "請確認資料是否完整", "確定");
                return;
            }

            if (Pay_method.Val == 0)
            {
                SubmitRequest();
            }
            else
            {
                var response = await service.CheckInviterRequest(Inviter_id_code);
                switch (response.Item1)
                {
                    case 1:
                        var result = await App.Current.MainPage.DisplayAlert("確認推薦人", $"確認推薦人姓名為：{response.Item2}", "是", "否");
                        if (result == true)
                        {
                            SubmitRequest();
                        }
                        break;
                    case 2:
                    case 3:
                        await App.Current.MainPage.DisplayAlert("錯誤", response.Item2, "確定");
                        break;
                    default:
                        break;
                }
            }
            
        }


        private async void GetDistrict()
        {

            var response = await service.GetDistrict();
            switch (response.Item1)
            {
                case 1:
                    List<District> dList = response.Item2;
                    foreach (var dis in dList)
                    {
                        DistrictList.Add(dis);
                    }
                    break;
                case 2:
                    await App.Current.MainPage.DisplayAlert("錯誤", "系統錯誤。", "確定");
                    break;
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", "伺服器無回應，網路連線錯誤。", "確定");
                    break;
                default:
                    break;
            }

        }


        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Inviter_id_code = parameters["Id"].ToString();
        }


        private bool isValid()
        {

            
            var result = true;
            Email_alert = false;
            Password_alert = false;
            Con_password_alert = false;
            Name_alert = false;
            Gender_alert = false;
            Birthdate_alert = false;
            Id_number_alert = false;
            District_alert = false;
            Address_alert = false;
            Pay_method_alert = false;
            Inviter_id_code_alert = false;

            if (String.IsNullOrEmpty(Email))
            {
                Email_alert = true;
                result = false;
            }
            if (String.IsNullOrEmpty(Password))
            {
                Password_alert = true;
                result = false;
            }
            if (Password != Con_password)
            {
                Con_password_alert = true;
                result = false;
            }
            if (String.IsNullOrEmpty(Name))
            {
                Name_alert = true;
                result = false;
            }
            if (Gender == null)
            {
                Gender_alert = true;
                result = false;
            }
            if (String.IsNullOrEmpty(Birthdate))
            {
                Birthdate_alert = true;
                result = false;
            }
            if (String.IsNullOrEmpty(Id_number))
            {
                Id_number_alert = true;
                result = false;
            }
            if (District == null)
            {
                District_alert = true;
                result = false;
            }
            if (String.IsNullOrEmpty(Address))
            {
                Address_alert = true;
                result = false;
            }
            if (Pay_method == null)
            {
                Pay_method_alert = true;
                result = false;
            }
            else
            {
                if (Pay_method.Val == 1)
                {
                    if (String.IsNullOrEmpty(Inviter_id_code))
                    {
                        Inviter_id_code_alert = true;
                        result = false;
                    }
                }
            }
            


            return result;
        }


        //------------------------------alert property------------------------------

        private bool email_alert = false;
        public bool Email_alert
        {
            get
            {
                return email_alert;
            }
            set
            {
                email_alert = value;
                OnPropertyChanged("Email_alert");

            }
        }

        private bool password_alert = false;
        public bool Password_alert
        {
            get
            {
                return password_alert;
            }
            set
            {
                password_alert = value;
                OnPropertyChanged("Password_alert");

            }
        }
        private bool con_password_alert = false;
        public bool Con_password_alert
        {
            get
            {
                return con_password_alert;
            }
            set
            {
                con_password_alert = value;
                OnPropertyChanged("Con_password_alert");

            }
        }
        private bool name_alert = false;
        public bool Name_alert
        {
            get
            {
                return name_alert;
            }
            set
            {
                name_alert = value;
                OnPropertyChanged("Name_alert");

            }
        }

        private bool gender_alert = false;
        public bool Gender_alert
        {
            get
            {
                return gender_alert;
            }
            set
            {
                gender_alert = value;
                OnPropertyChanged("Gender_alert");

            }
        }

        private bool birthdate_alert = false;
        public bool Birthdate_alert
        {
            get
            {
                return birthdate_alert;
            }
            set
            {
                birthdate_alert = value;
                OnPropertyChanged("Birthdate_alert");

            }
        }

        private bool id_number_alert = false;
        public bool Id_number_alert
        {
            get
            {
                return id_number_alert;
            }
            set
            {
                id_number_alert = value;
                OnPropertyChanged("Id_number_alert");

            }
        }

        private bool district_alert = false;
        public bool District_alert
        {
            get
            {
                return district_alert;
            }
            set
            {
                district_alert = value;
                OnPropertyChanged("District_alert");

            }
        }

        private bool address_alert = false;
        public bool Address_alert
        {
            get
            {
                return address_alert;
            }
            set
            {
                address_alert = value;
                OnPropertyChanged("Address_alert");

            }
        }

        private bool pay_method_alert = false;
        public bool Pay_method_alert
        {
            get
            {
                return pay_method_alert;
            }
            set
            {
                pay_method_alert = value;
                OnPropertyChanged("Pay_method_alert");

            }
        }

        private bool inviter_id_code_alert = false;
        public bool Inviter_id_code_alert
        {
            get
            {
                return inviter_id_code_alert;
            }
            set
            {
                inviter_id_code_alert = value;
                OnPropertyChanged("Inviter_id_code_alert");

            }
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
