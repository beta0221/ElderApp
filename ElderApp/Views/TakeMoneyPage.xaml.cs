using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;


namespace ElderApp.Views
{
    public partial class TakeMoneyPage : ContentPage
    {

        public TakeMoneyPage()
        {
            InitializeComponent();

            string text = App.CurrentUser.User_id + "," + App.CurrentUser.Name + "," + App.CurrentUser.Email;
            UserBarcode.BarcodeValue = text;
               
        }
    }
}
