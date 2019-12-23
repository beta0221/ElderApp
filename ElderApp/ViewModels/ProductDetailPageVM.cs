using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ElderApp.Models;
using ElderApp.Services;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace ElderApp.ViewModels
{
    public class ProductDetailPageVM : INotifyPropertyChanged, INavigationAware
    {
        INavigationService _navigationService;

        private Product selectProduct { get; set; }
        public Product SelectProduct
        {
            get { return selectProduct; }
            set
            {
                selectProduct = value;
                if(selectProduct != null)
                {
                    GetLocationAndQuantity(selectProduct.slug);
                }
                OnPropertyChanged("SelectProduct");
            }
        }

        private int locationViewHeight { get; set; }
        public int LocationViewHeight
        {
            get { return locationViewHeight; }
            set
            {
                locationViewHeight = value;
                OnPropertyChanged("LocationViewHeight");
            }
        }

        private Dictionary<int,Boolean> LocationDic { get; set; }

        private ObservableCollection<Location> locations { get; set; }
        public ObservableCollection<Location> Locations
        {
            get
            {
                return locations;
            }
            set
            {
                locations = value;
                OnPropertyChanged("Locations");
            }
        }

        private Location selectLocation { get; set; }
        public Location SelectLocation
        {
            get { return selectLocation; }
            set
            {
                selectLocation = value;
                OnPropertyChanged("SelectLocation");
            }
        }

        private Location selectLocationDetail { get; set; }
        public Location SelectLocationDetail
        {
            get { return selectLocationDetail; }
            set
            {
                selectLocationDetail = value;
                if (selectLocationDetail!=null)
                {
                    ShowLocationDetailRequest();
                }
                OnPropertyChanged("SelectLocationDetail");
            }
        }

        private async void GetLocationAndQuantity(string slug)
        {
            var response = await service.GetLocationAndQuantity(slug);
            switch (response.Item1)
            {
                case 1:
                    var res = response.Item2;
                    foreach (var item in res)
                    {
                        LocationDic.Add(item.location_id, true);
                    }
                    GetAllLocation();
                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", "系統錯誤", "確定");
                    break;
            }
        }

        private async void GetAllLocation()
        {
            var response = await service.GetLocation();
            switch (response.Item1)
            {
                case 1:
                    var res = response.Item2;
                    foreach(var item in res)
                    {
                        if (LocationDic.ContainsKey(item.id))
                        {
                            Locations.Add(item);
                            LocationViewHeight += 56;
                        }
                    }
                    break;
                case 2:
                case 3:
                    break;
            }
        }

        public ICommand Purchase { get; set; }
        public ICommand HideLocationDetail { get; set; }
        public ICommand ShowOnMap { get; set; }

        private string _Lname;
        public string Lname
        {
            get
            {
                return _Lname;
            }
            set
            {
                _Lname = value;
                OnPropertyChanged("Lname");
            }
        }
        private string _Laddress;
        public string Laddress
        {
            get
            {
                return _Laddress;
            }
            set
            {
                _Laddress = value;
                OnPropertyChanged("Laddress");
            }
        }

        private bool isLocationVisable;
        public bool IsLocationVisable
        {
            get { return isLocationVisable; }
            set
            {
                isLocationVisable = value;
                OnPropertyChanged("IsLocationVisable");
            }
        }

        private ApiServices service;
        public ProductDetailPageVM(INavigationService navigationService)
        {
            _navigationService = navigationService;
            service = new ApiServices();
            LocationDic = new Dictionary<int, bool>();
            Locations = new ObservableCollection<Location>();
            LocationViewHeight = 0;

            Purchase = new DelegateCommand(PurchaseRequest);
            HideLocationDetail = new DelegateCommand(HideLocationDetailRequest);
            ShowOnMap = new DelegateCommand(ShowOnMapRequest);
            IsLocationVisable = false;

        }

        private async void ShowOnMapRequest()
        {
            var url = SelectLocationDetail.link;
            Device.OpenUri(new Uri(url));
        }

        private async void HideLocationDetailRequest()
        {
            IsLocationVisable = false;
        }

        private async void ShowLocationDetailRequest()
        {
            
            Lname = SelectLocationDetail.Lname;
            Laddress = SelectLocationDetail.Laddress;
            SelectLocationDetail = null;
            IsLocationVisable = true;
        }

        private async void PurchaseRequest()
        {
            if (SelectLocation == null)
            {
                await App.Current.MainPage.DisplayAlert("訊息", "請先選擇兌換據點", "確定");
            }
            else
            {
                var response = await service.PurchaseProduct(SelectLocation.id, SelectProduct.slug);
                switch (response.Item1)
                {
                    case 1:
                        var res = response.Item2;
                        if (res != "success")
                        {
                            await App.Current.MainPage.DisplayAlert("訊息", res, "確定");
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("訊息", "兌換成功", "確定");
                        }
                        break;
                    case 2:
                    case 3:
                        await App.Current.MainPage.DisplayAlert("錯誤", "系統錯誤", "確定");
                        break;

                }
            }
            

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
            if (parameters["product"] != null)
            {
                SelectProduct = parameters["product"] as Product;
            }
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            
        }

        
    }
}
