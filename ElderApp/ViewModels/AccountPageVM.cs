using System;
using System.ComponentModel;
using System.Windows.Input;
using ElderApp.Helpers;
using ElderApp.Models;
using ElderApp.Services;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;
using SQLite;
using Xamarin.Forms;

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

        private string id_code;
        public string Id_code
        {
            get { return id_code; }
            set
            {
                id_code = value;
                OnPropertyChanged("Id_code");
            }
        }



        public ICommand Edit { get; set; }

        public ICommand Logout { get; set; }

        public ICommand ExtendMembership { get; set; }

        public ICommand SelectImage { get; set; }

        public AccountPageVM(INavigationService navigationService)
        {
            
            _navigationService = navigationService;
            Edit = new DelegateCommand(EditRequest);
            Logout = new DelegateCommand(LogoutRequest);
            ExtendMembership = new DelegateCommand(ExtendRequest);
            SelectImage = new DelegateCommand(SelectImageRequest);

        }


        private async void EditRequest()
        {
            await _navigationService.NavigateAsync("EditAccountPage");
        }

        private async void ExtendRequest()
        {
            System.Diagnostics.Debug.WriteLine("Extend Request");

            var service = new ApiServices();
            var response = await service.ExtendRequest();

            switch (response.Item1)
            {
                case 1:
                    await App.Current.MainPage.DisplayAlert("訊息", response.Item2, "確定");
                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", response.Item2, "確定");
                    break;
                default:
                    break;
            }

            

        }

        private async void MyAccountRequest()
        {
            System.Diagnostics.Debug.WriteLine("MyAccount");

            var service = new ApiServices();
            var response = await service.MyAccountRequest();
            switch (response.Item1)
            {
                case 1:
                    var res = response.Item2;
                    if (res.ContainsKey("img"))
                    {
                        var userId = App.CurrentUser.User_id;
                        string image_url = "";
                        System.Diagnostics.Debug.WriteLine(res["img"].ToString());
                        if (String.IsNullOrEmpty(res["img"].ToString()))
                        {

                            image_url = "user_default.png";
                        }
                        else
                        {
                            image_url = $"{service.ApiHost}/images/users/{userId}/{res["img"].ToString()}";
                        }

                        Image_url = image_url;
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
                            Valid = "無效";
                            Valid_color = "#E22600";
                            Extend = true;
                        }

                        Id_code = res["id_code"].ToString();

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

        private async void LogoutRequest()
        {

            var result = await App.Current.MainPage.DisplayAlert("登出使用者", "確定登出", "是", "否");
            if (result == true)
            {

                System.Diagnostics.Debug.WriteLine("Logout");

                var service = new ApiServices();
                var response = await service.LogoutRequest();

                switch (response)
                {
                    case 1:
                    case 2:
                        using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                        {
                            conn.Execute("DELETE FROM UserModel");
                        }
                        await _navigationService.NavigateAsync("/LoginPage");
                        break;
                    
                    case 3:
                        await App.Current.MainPage.DisplayAlert("錯誤", "伺服器無回應，網路連線錯誤。", "確定");
                        break;
                    default:
                        break;
                }

            }

        }

        private async void SelectImageRequest()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await App.Current.MainPage.DisplayAlert("Not able", "Not able to pick photo", "OK");
                return;
            }

            //pick image
            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
                return;

            var stream = file.GetStream();

            byte[] byteArray = ImageConverter.StreamToByteArray(stream);
            string image_string = Convert.ToBase64String(byteArray);

            var service = new ApiServices();
            var response = await service.UploadImageRequest(image_string);
            switch (response.Item1)
            {
                case 1:
                    var image_name = response.Item2;
                    string img_url = "";
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
                    {
                        var _user = conn.Table<UserModel>().FirstOrDefault();
                        img_url = $"{service.ApiHost}/images/users/{_user.User_id}/{image_name}";
                        conn.Execute($"UPDATE UserModel SET Img = '{img_url}' WHERE Id = {_user.Id}");
                    }
                    Image_url = img_url;
                    App.CurrentUser.Img = img_url;
                    
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

        public void OnAppearing()
        {
            MyAccountRequest();
        }

        public void OnDisappearing()
        {
            
        }
    }
}
