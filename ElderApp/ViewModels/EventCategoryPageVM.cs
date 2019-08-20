using System;

using Xamarin.Forms;

namespace ElderApp.ViewModels
{
    public class EventCategoryPageVM : ContentPage
    {
        public EventCategoryPageVM()
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

