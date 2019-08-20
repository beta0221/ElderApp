using System;
using System.Collections.Generic;
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

        public EditAccountPageVM(INavigationService navigationService)
        {
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

            var client = new RestClient("http://128.199.197.142/api/auth/updateAccount");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token.ToString());
            request.AddParameter("name", Name);
            request.AddParameter("phone", Phone);
            request.AddParameter("tel", Tel);
            request.AddParameter("address", Address);
            request.AddParameter("id_number", Id_number);
            IRestResponse response = client.Execute(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);
                    
                    if(res["s"].ToString() == "1")
                    {
                        await App.Current.MainPage.DisplayAlert("完成", res["m"].ToString(), "確定");
                        await _navigationService.NavigateAsync("/NavigationPage/MyPage");
                    }
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Yes");
                }
            }

        }

        private async void MyAccountRequest()
        {
            System.Diagnostics.Debug.WriteLine("MyAccount");

            var client = new RestClient("http://128.199.197.142/api/auth/myAccount");
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
                    Name = res["name"].ToString();

                    //if (res["gender"].ToString() == "1")
                    //{
                    //    GenderItem = new Gender() { Key=1,Value="男"};
                    //}
                    //else
                    //{
                    //    GenderItem = new Gender() { Key = 0, Value = "女" };
                    //}

                    Phone = res["phone"].ToString();
                    Tel = res["tel"].ToString();
                    Address = res["address"].ToString();
                    Id_number = res["id_number"].ToString();

                    

                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Yes");
                    await _navigationService.NavigateAsync("/NavigationPage/MyPage");
                }
            }

        }


    }
}
