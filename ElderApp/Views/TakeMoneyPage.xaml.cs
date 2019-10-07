using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;


namespace ElderApp.Views
{
    public partial class TakeMoneyPage : ContentPage
    {

        public TakeMoneyPage()
        {
            InitializeComponent();

            string text = App.CurrentUser.User_id + "," + StringToUnicode(App.CurrentUser.Name) + "," + App.CurrentUser.Email;
            UserBarcode.BarcodeValue = text;
               
        }



        private string StringToUnicode(string srcText)
        {
            string dst = "";
            char[] src = srcText.ToCharArray();
            for (int i = 0; i < src.Length; i++)
            {
                byte[] bytes = Encoding.Unicode.GetBytes(src[i].ToString());
                string str = @"\u" + bytes[1].ToString("X2") + bytes[0].ToString("X2");
                dst += str;
            }
            return dst;
        }

    }

    

}
