using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Apollon.Common
{
    class TimeSpanToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(double))
                return null;
            var timeSpan = value as TimeSpan?;

            return timeSpan?.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!(value is double))
                return null;
            return TimeSpan.FromSeconds((double)value);
        }
    }
}
