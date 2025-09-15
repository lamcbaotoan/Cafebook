using System;
using System.Globalization;
using System.Windows.Data;

namespace Cafebook // Đảm bảo namespace này khớp với namespace gốc của project bạn
{
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}