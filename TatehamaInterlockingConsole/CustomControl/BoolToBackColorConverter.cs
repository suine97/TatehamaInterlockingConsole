using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TatehamaInterlockingConsole.CustomControl
{
    public class BoolToBackColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                // #FF000000形式の色を設定
                var colorCode = boolean ? "#FF67FF4C" : "#FF555555";
                return (SolidColorBrush)new BrushConverter().ConvertFromString(colorCode);
            }
            return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFFFF");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
