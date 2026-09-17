using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RestaurantPOS.Converters
{
    public class StatusToTextColorConverter : IValueConverter
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
                "Available" => new SolidColorBrush(Color.FromRgb(21, 128, 61)),
                "Occupied" => new SolidColorBrush(Color.FromRgb(185, 28, 28)),
                "Reserved" => new SolidColorBrush(Color.FromRgb(161, 98, 7)),
                _ => new SolidColorBrush(Colors.Gray)
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