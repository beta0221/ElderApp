using System;

using Xamarin.Forms;

namespace ElderApp.ViewModels
{
    public class MyTabbedPageVM : ContentView
    {
        public MyTabbedPageVM()
        {
            Content = new Label { Text = "Hello ContentView" };
        }
    }
}

