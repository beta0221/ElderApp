using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ElderApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.AppModel;
using RestSharp;
using Xamarin.Forms;
using SQLite;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;


namespace ElderApp.Views
{
    public partial class EventDetailPage : ContentPage
    {
        public EventDetailPage()
        {
            InitializeComponent();
        }
    }
}
