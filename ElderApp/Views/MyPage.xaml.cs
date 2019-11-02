using System;
using System.Collections.Generic;
using System.IO;
using ElderApp.Helpers;
using Plugin.Media;
using Xamarin.Forms;

namespace ElderApp.Views
{
    public partial class MyPage : ContentPage
    {
        public MyPage()
        {
            InitializeComponent();

            
        }

        private void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            sliderBG.IsVisible = false;
        }
    }
}
