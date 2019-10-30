using System;
using System.Collections.Generic;
using ElderApp.Services;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;

namespace ElderApp.ViewModels
{
    public class EditAccountPageVM : INotifyPropertyChanged
    {
        INavigationService _navigationService;

        public event PropertyChangedEventHandler PropertyChanged;

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

        //private Gender genderItem;
        //public Gender GenderItem
        //{ get
        //    {
        //        return genderItem;
        //    }
        //    set
        //    {
        //        genderItem = value;
        //        OnPropertyChanged("GenderItem");
        //    }
        //}




        //private string birthdate;
        //public string Birthdate
        //{
        //    get { return birthdate; }
        //    set {
        //        birthdate = value;
        //        OnPropertyChanged("Birthdate");
        //    }
        //}

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

        public ICommand Cancel { get; set; }

        public ICommand Submit { get; set; }

        //public List<Gender> GenderList
        //{
        //    get
        //    {
        //        var gender = new List<Gender>()
        //        {
        //            new Gender(){Key=0,Value="女"},
        //            new Gender(){Key=1,Value="男"}
        //        };
        //        return gender;
        //    }
        //}

        //public class Gender
        //{
        //    public int Key { get; set; }
        //    public string Value { get; set; }
        //}

        private ApiServices service;
        public EditAccountPageVM(INavigationService navigationService)
        {
            service = new ApiServices();
            _navigationService = navigationService;
            Cancel = new DelegateCommand(CancelRequest);
            Submit = new DelegateCommand(SubmitRequest);
            MyAccountRequest();

        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        private async void CancelRequest()
        {
            await _navigationService.GoBackAsync();
        }

        private async void SubmitRequest()
        {
            System.Diagnostics.Debug.WriteLine("Submit Edit Request");

            var response = await service.UpdateAccountRequest(Name,Phone,Tel,Address,Id_number);
            switch (response.Item1)
            {
                case 1:
                    await App.Current.MainPage.DisplayAlert("完成", response.Item2, "確定");
                    await _navigationService.NavigateAsync("/FirstPage?selectedTab=AccountPage");
                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤",response.Item2 , "確定");
                    break;
                default:
                    break;
            }


        }

        private async void MyAccountRequest()
        {
            System.Diagnostics.Debug.WriteLine("MyAccount");

            var response = await service.MyAccountRequest();

            switch (response.Item1)
            {
                case 1:
                    var res = response.Item2;
                    Name = res["name"].ToString();
                    Phone = res["phone"].ToString();
                    Tel = res["tel"].ToString();
                    Address = res["address"].ToString();
                    Id_number = res["id_number"].ToString();
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


    }
}
