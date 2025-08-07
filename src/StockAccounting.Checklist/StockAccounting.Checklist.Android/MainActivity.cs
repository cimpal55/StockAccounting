using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Acr.UserDialogs;
using System.Net;
using Xamarin.Forms;

namespace StockAccounting.Checklist.Droid
{

    [Activity(Label = "StockAccounting", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            UserDialogs.Init(this);
            ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
            Forms.SetFlags("FastRenderers_Experimental");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            //ZXing.Net.Mobile.Forms.Android.Platform.Init();
            //ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);
            Xamarin.KeyboardHelper.Platform.Droid.Effects.Init(this);
            Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}