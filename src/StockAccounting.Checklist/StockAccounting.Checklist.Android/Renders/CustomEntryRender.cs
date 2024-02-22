using StockAccounting.Checklist.Controls;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Content;
using Xamarin.Forms.Material.Android;
using StockAccounting.Checklist.Droid.Renders;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(CustomEntryRenderer))]
namespace StockAccounting.Checklist.Droid.Renders
{
    public class CustomEntryRenderer : MaterialEntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context) { }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.EditText.Background = null;
                Control.EditText.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}