using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ElderApp.Views
{
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();

            
            if (String.IsNullOrEmpty(App.CurrentUser.Id_code))
            {
                UserIdCode.IsVisible = false;
                UserIdCode.BarcodeValue = "0000000000";
            }
            else
            {
                string id_code = App.CurrentUser.Id_code;
                UserIdCode.BarcodeValue = id_code;
                UserIdCode.IsVisible = true;

            }



        }
    }
}
