using System;
using ElderApp.Models;
using ElderApp.ViewModels;
using ElderApp.Views;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElderApp
{
    public partial class App : PrismApplication
    {
        public static string DatabasePath;
        public static UserModel CurrentUser;

        public App(IPlatformInitializer initializer = null):base(initializer)
        {


        }

        public App(string databasePath,IPlatformInitializer initializer = null) : base(initializer)
        {
            DatabasePath = databasePath;



            //NavigationService.NavigateAsync("NavigationPage/MyPage");

            using (SQLiteConnection conn = new SQLiteConnection(DatabasePath))
            {
                try
                {
                    UserModel user = conn.Table<UserModel>().FirstOrDefault();
                    if (user != null)
                    {
                        CurrentUser = user;

                        NavigationService.NavigateAsync("NavigationPage/MyPage");
                    }
                    else
                    {

                        //UserModel createUser = new UserModel()
                        //{
                        //    Id = 1,
                        //    Name = "user",
                        //    Email = "user@user.com",
                        //    Wallet = 10000,
                        //    Rank = 1,
                        //    Img = "http://127.0.0.1:8000/images/users/user/1557835651.png",
                        //    Token = "blablabla"
                        //};

                        //conn.CreateTable<UserModel>();

                        //int insertedd = conn.Insert(createUser);
                        //if(insertedd >= 1)
                        //{
                        //    App.CurrentUser = createUser;
                        //    NavigationService.NavigateAsync("NavigationPage/MyPage");
                        //}

                        NavigationService.NavigateAsync("/LoginPage");
                    }
                }
                catch (Exception ex)
                {
                    NavigationService.NavigateAsync("/LoginPage");
                }

            }



        }

        protected override void OnInitialized()
        {
            InitializeComponent();


        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MyPage,MyPageVM>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageVM>();
            containerRegistry.RegisterForNavigation<TakeMoneyPage, TakeMoneyPageVM>();
            containerRegistry.RegisterForNavigation<ScannerPage, ScannerPageVM>();
            containerRegistry.RegisterForNavigation<TransHistoryPage, TransHistoryPageVM>();
            containerRegistry.RegisterForNavigation<GiveMoneyPage, GiveMoneyPageVM>();

        }


    }
}
