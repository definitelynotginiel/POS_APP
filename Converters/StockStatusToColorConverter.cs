using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace POS_APP.Converters
{
    public class StockStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status)
                {
                    case "Good Stock":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2ECC71"));
                    case "Low Stock":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1C40F"));
                    case "Out of Stock":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE74C3C"));
                    default:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF95A5A6"));
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
