using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SomwApp.presentation.converters
{
    public class ReviewTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is ReviewType reviewType)
            {
                switch (reviewType)
                {
                    case ReviewType.Attendance:
                        return "Посещаемость";
                    case ReviewType.Income:
                        return "Доходов";
                    case ReviewType.Branch_Efficiency:
                        return "Доходность филлиала";
                    case ReviewType.Trainer_Business:
                        return "Загруженность тренера";
                }
            }
            return "Нет информации";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is string str)
            {
                switch (str)
                {
                    case  "Посещаемость":
                        return ReviewType.Attendance;
                    case "Доходов":
                        return ReviewType.Income;
                    case "Доходность филлиала":
                        return ReviewType.Branch_Efficiency;
                    case "Загруженность тренера":
                        return ReviewType.Trainer_Business;
                }
            }
            return ReviewType.Income;
        }
    }

    public class ReviewTypesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var newList = new List<string>();
            if (value != null && value is List<ReviewType> types)
            {
                foreach (var type in types)
                    switch (type)
                    {
                        case ReviewType.Attendance:
                            newList.Add("Посещаемость");
                            break;
                        case ReviewType.Income:
                            newList.Add("Доходов");
                            break;
                        case ReviewType.Branch_Efficiency:
                            newList.Add("Доходность филлиала");
                            break;
                        case ReviewType.Trainer_Business:
                            newList.Add("Загруженность тренера");
                            break;
                    }
            }
            return newList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var newList = new List<ReviewType>();
            if (value != null && value is List<string> strs)
            {
                foreach (var str in strs)
                    switch (str)
                    {
                        case "Посещаемость":
                            newList.Add( ReviewType.Attendance);
                            break;
                        case "Доходов":
                            newList.Add(ReviewType.Income);
                            break;
                        case "Доходность филлиала":
                            newList.Add(ReviewType.Branch_Efficiency);
                            break;
                        case "Загруженность тренера":
                            newList.Add(ReviewType.Trainer_Business);
                            break;
                    }
            }
            return newList;
        }
    }
}
