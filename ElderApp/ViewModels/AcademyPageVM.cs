using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Essentials;

namespace ElderApp.ViewModels
{
    public class AcademyPageVM
    {
        INavigationService _navigationService;


        public ICommand Events { get; set; }        //活動

        public ICommand My_events { get; set; }     //我的活動

        public double SliderHeight { get; set; }

        public AcademyPageVM(INavigationService navigationService)
        {
            Events = new DelegateCommand(EventsRequest);        //活動
            My_events = new DelegateCommand(My_eventsRequest);
            _navigationService = navigationService;

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var density = mainDisplayInfo.Density;
            var screenWidth = mainDisplayInfo.Width / density;
            SliderHeight = screenWidth * 0.75;
        }


        private async void EventsRequest()                      //活動
        {
            await _navigationService.NavigateAsync("EventPage");
        }

        private async void My_eventsRequest()                      //我的活動
        {
            await _navigationService.NavigateAsync("MyEventPage");
        }

    }

    
}
