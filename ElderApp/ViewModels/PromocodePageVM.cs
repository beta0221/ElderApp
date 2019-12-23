using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ElderApp.Models;
using ElderApp.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using RestSharp;

namespace ElderApp.ViewModels
{
    public class PromocodePageVM : INotifyPropertyChanged
    {
        INavigationService _navigationService;

        public ObservableCollection<Product> products { get; set; }
        public ObservableCollection<Order> orders { get; set; }

        private bool isMyListVisable;
        public bool IsMyListVisable
        {
            get { return isMyListVisable; }
            set
            {
                isMyListVisable = value;
                OnPropertyChanged("IsMyListVisable");
            }
        }

        private string leftBtnColor { get; set; }
        public string LeftBtnColor
        {
            get { return leftBtnColor; }
            set
            {
                leftBtnColor = value;
                OnPropertyChanged("LeftBtnColor");
            }
        }

        private string rightBtnColor { get; set; }
        public string RightBtnColor
        {
            get { return rightBtnColor; }
            set
            {
                rightBtnColor= value;
                OnPropertyChanged("RightBtnColor");
            }
        }

        private Product selectProduct { get; set; }
        public Product SelectProduct
        {
            get{return selectProduct;}
            set
            {
                selectProduct = value;
                if (selectProduct!=null)
                {
                    GoToDetailPage(selectProduct);
                }
                OnPropertyChanged("SelectProduct");
            }
        }

        public async void GoToDetailPage(Product selectProduct)
        {
            SelectProduct = null;
            var navigationParams = new NavigationParameters();
            navigationParams.Add("product", selectProduct);
            await _navigationService.NavigateAsync("ProductDetailPage", navigationParams);
        }

        public ICommand showAllProduct { get; set; }
        public ICommand showMyList { get; set; }

        private ApiServices service;
        public PromocodePageVM(INavigationService navigationService)
        {
            service = new ApiServices();
            _navigationService = navigationService;
            IsMyListVisable = false;
            products = new ObservableCollection<Product>();
            orders = new ObservableCollection<Order>();
            showAllProduct = new DelegateCommand(ShowAllProduct);
            showMyList = new DelegateCommand(ShowMyList);
            ShowAllProduct();
            GetAllProducts();

        }

        private void ShowAllProduct()
        {
            IsMyListVisable = false;
            LeftBtnColor = "#FE7235";
            RightBtnColor = "#FEA735";
        }

        private async void ShowMyList()
        {
            LeftBtnColor = "#FEA735";
            RightBtnColor = "#FE7235";
            
            IsMyListVisable = true;
            orders.Clear();
            var response = await service.MyOrderList();
            switch (response.Item1)
            {
                case 1:
                    var res = response.Item2;
                    foreach (var item in res)
                    {
                        orders.Add(item);
                    }
                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", "系統錯誤", "確定");
                    break;
            }
        }

        private async void GetAllProducts()
        {
            var response = await service.GetAllProducts();
            switch (response.Item1)
            {
                case 1:
                    ProductAndCat res = response.Item2;

                    foreach (var p in res.products)
                    {
                        products.Add(p);
                    }
                    

                    break;
                case 2:
                case 3:
                    await App.Current.MainPage.DisplayAlert("錯誤", "系統錯誤", "確定");
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        






    }
}
