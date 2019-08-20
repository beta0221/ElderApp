using System;
using Newtonsoft.Json.Linq;
using Prism.Navigation;
using RestSharp;

namespace ElderApp.ViewModels
{
    public class AccountPageVM
    {
        INavigationService _navigationService;

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


        public AccountPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            MyAccountRequest();
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
                    await _navigationService.GoBackAsync();
                }
            }

        }
    }
}
