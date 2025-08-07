using System.Globalization;
using StockAccounting.Core.Android.Enums;

namespace StockAccounting.Inventory.Utils.Converters
{
    public class StatusToColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            string color;

            if (s == InventoryStatus.InProcess.ToString())
                color = "Pink";
            else
                color = "Yellow";

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}