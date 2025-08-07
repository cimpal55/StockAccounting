using System.Globalization;

namespace StockAccounting.Inventory.Utils.Converters
{
    public sealed class StringToDecimalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is decimal decimalValue
                ? decimalValue.ToString(CultureInfo.InvariantCulture)
                : "0";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string stringValue)
                return 0m;

            return decimal.TryParse(stringValue, out var decimalValue)
                ? decimalValue
                : 0m;
        }
	}
}
