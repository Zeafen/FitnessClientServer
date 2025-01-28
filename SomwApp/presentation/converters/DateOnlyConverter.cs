using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SomwApp.presentation.converters
{
    public class DateOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is DateOnly date)
            {
                return date.ToDateTime(new TimeOnly(0));
            }
            return DateTime.Now;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is DateTime time)
            {
                return DateOnly.FromDateTime(time);
            }
            return DateOnly.FromDateTime(DateTime.Now);
        }
    }
    public class TimeOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is TimeOnly time)
            {
                return DateOnly.FromDateTime(DateTime.Now).ToDateTime(time);
            }
            return DateTime.Now;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is DateTime dateTime)
            {
                return TimeOnly.FromDateTime(dateTime);
            }
            return TimeOnly.FromDateTime(DateTime.Now);
        }
    }
}
