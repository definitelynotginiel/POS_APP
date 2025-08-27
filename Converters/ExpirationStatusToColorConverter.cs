using FontAwesome.WPF;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace POS_APP.Converters;
public class ExpirationStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status switch
            {
                "Expired" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE74C3C")),
                "Near Expiry" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF39C12")),
                "Good" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF27AE60")),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ExpirationStatusToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status switch
            {
                "Expired" => FontAwesomeIcon.ExclamationCircle,
                "Near Expiry" => FontAwesomeIcon.ExclamationTriangle,
                "Good" => FontAwesomeIcon.CheckCircle,
                _ => FontAwesomeIcon.QuestionCircle
            };
        }
        return FontAwesomeIcon.QuestionCircle;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}