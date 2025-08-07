using System.Globalization;

namespace StockAccounting.Inventory.Utils.Converters
{
    public class BoolToColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isQuantitiesEqual)
            {
                return isQuantitiesEqual ? Colors.LightGreen : Colors.Transparent;
            }
            else
            {
                return Colors.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}