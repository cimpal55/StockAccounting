using Android.Content;
using Android.Support.V4.Content.Res;
using Android.Widget;
using StockAccounting.Checklist.Droid.Renders;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(NoRippleListViewRenderer))]
namespace StockAccounting.Checklist.Droid.Renders
{
    public class NoRippleListViewRenderer : ListViewRenderer
    {
        public NoRippleListViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            Control.SetSelector(Resource.Drawable.listview_cell);
        }
    }
}