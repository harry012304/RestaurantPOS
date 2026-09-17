using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RestaurantPOS.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            string status = value?.ToString() ?? string.Empty;

            return status switch
            {
                "Available" => new SolidColorBrush(Color.FromRgb(220, 252, 231)),
                "Occupied" => new SolidColorBrush(Color.FromRgb(254, 226, 226)),
                "Reserved" => new SolidColorBrush(Color.FromRgb(254, 249, 195)),
                _ => new SolidColorBrush(Colors.LightGray)
            };
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}