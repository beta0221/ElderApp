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

namespace ElderApp.ViewModels
{
    public class EventDetailPageVM : INotifyPropertyChanged, INavigationAware
    {
        private Event select_event { get; set; }
        public Event Select_event
        {
            get { return select_event; }
            set
            {
                select_event = value;
                OnPropertyChanged(nameof(Select_event));
            }
        }


        public EventDetailPageVM(Event eve)
        {

        }



        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Select_event = parameters["eve"] as Event;
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

