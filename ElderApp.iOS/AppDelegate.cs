using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;
using ImageCircle.Forms.Plugin.iOS;


namespace ElderApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            

            // circle image
            ImageCircleRenderer.Init();
            // circle image

            //ZXing
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            //ZXing

            //sqlite
            string fileName = "ElderApp.db3";
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"..","Library");
            string completePath = Path.Combine(folderPath, fileName);
            //sqlite




            LoadApplication(new App(completePath,new iOSInitializer()));

            return base.FinishedLaunching(app, options);
        }


        public class iOSInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {

            }
        }
    }
}
