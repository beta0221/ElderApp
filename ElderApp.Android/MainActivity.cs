using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Prism;
using Prism.Ioc;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Provider;
using Android.Database;
using ImageCircle.Forms.Plugin.Droid;


namespace ElderApp.Droid
{

    [Activity(Label = "ElderApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        public static MainActivity Instance { get; private set; }

        public MainActivity()
        {
            Instance = this;
        }







        // Field, property, and method for Picture Picker

        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == PickImageId)
            {


                if ((resultCode == Result.Ok) && (data != null))
                {
                    Android.Net.Uri uri = data.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);

                    //string path = GetFilePath(uri);

                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }


            }
        }

        //private string GetFilePath(Android.Net.Uri uri)
        //{
        //    string filePath = "";
        //    //             
        //    string imageId = DocumentsContract.GetDocumentId(uri);
        //    string id = imageId.Split(':')[1];
        //    string[] proj = { MediaStore.Images.Media.InterfaceConsts.Data };
        //    string sel = MediaStore.Images.Media.InterfaceConsts.Id + "=?";



        //    using (ICursor cursor = ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, proj, sel, new string[] { id }, null))
        //    {
        //        int columnIndex = cursor.GetColumnIndex(proj[0]);
        //        if (cursor.MoveToFirst())
        //        {
        //            filePath = cursor.GetString(columnIndex);
        //        }
        //    }
        //    return filePath;

        //}

        // Field, property, and method for Picture Picker



        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            

            //image circle
            ImageCircleRenderer.Init();
            //image circle

            //ZXing
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            //ZXing

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);




            //sqlite
            string fileName = "ElderApp.db3";
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string completePath = Path.Combine(folderPath, fileName);
            //sqlite

           

            LoadApplication(new App(completePath,new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {

            }
        }
    }
}