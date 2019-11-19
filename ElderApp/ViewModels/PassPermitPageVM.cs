using System;
using System.ComponentModel;
using Prism.Navigation;

namespace ElderApp.ViewModels
{
    public class PassPermitPageVM: INotifyPropertyChanged, INavigatedAware
    {

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }


        public PassPermitPageVM()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Title = parameters["name"].ToString();
        }
    }
}
