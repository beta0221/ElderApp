using System;
using System.Collections.Generic;
using ElderApp.Models;
using ElderApp.ViewModels;
using ElderApp.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
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

            using (SQLiteConnection conn = new SQLiteConnection(DatabasePath))
            {
                try
                {
                    UserModel user = conn.Table<UserModel>().FirstOrDefault();
                    if (user != null)
                    {
                        CurrentUser = user;

                        //NavigationService.NavigateAsync("NavigationPage/MyPage");
                        NavigationService.NavigateAsync("FirstPage");
                    }
                    else
                    {
                        NavigationService.NavigateAsync("LoginPage");
                    }


                    //conn.Execute("DELETE FROM UserModel");
                    //NavigationService.NavigateAsync("/LoginPage");
                }
                catch (Exception ex)
                {
                    NavigationService.NavigateAsync("LoginPage");

                    var properties = new Dictionary<string, string>
                    {
                        {"error","No User in Sqlite"}
                    };
                    Crashes.TrackError(ex, properties);
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

            containerRegistry.RegisterForNavigation<EventPage, EventPageVM>();
            containerRegistry.RegisterForNavigation<EventDetailPage, EventDetailPageVM>();
            containerRegistry.RegisterForNavigation<MyEventPage, MyEventPageVM>();

            containerRegistry.RegisterForNavigation<AccountPage, AccountPageVM>();
            containerRegistry.RegisterForNavigation<EditAccountPage, EditAccountPageVM>();
            //containerRegistry.RegisterForNavigation<CategoryPage, CategoryPageVM>();
            containerRegistry.RegisterForNavigation<FirstPage>();


        }


        protected override void OnStart()
        {
            string androidAppSecret = "1a282c36-cd4e-42a5-902e-c0e660ca2151";
            string iOSAppSecret = "ebf97172-1b0d-4728-ad56-5d17c5abf99d";
            AppCenter.Start($"android={androidAppSecret};ios={iOSAppSecret}", typeof(Crashes));
        }

    }
}
