using System;

using Xamarin.Forms;

namespace ElderApp.Views
{
    public class MyEventPage : ContentPage
    {
        public MyEventPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

