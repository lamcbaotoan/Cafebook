using System;
using System.Globalization;
using System.Windows.Data;

namespace Cafebook // Đảm bảo namespace này khớp với namespace gốc của project bạn
{
    public class IsLessThanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int threshold = 5; // Ngưỡng mặc định là 5
            if (parameter != null)
            {
                int.TryParse(parameter.ToString(), out threshold);
            }

            // Chuyển đổi giá trị binding (là SoLuongCoThePhucVu) sang số nguyên
            int intValue = System.Convert.ToInt32(value);

            // Trả về true nếu số lượng lớn hơn 0 VÀ nhỏ hơn hoặc bằng ngưỡng
            return intValue > 0 && intValue <= threshold;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}