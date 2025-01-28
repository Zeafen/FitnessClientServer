using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SomwApp.presentation.converters
{
    public class SubscriptionStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is SubscriptionStatus status)
            {
                switch (status)
                {
                    case SubscriptionStatus.Active:
                        return "Активный";
                    case SubscriptionStatus.Expired:
                        return "Истёк срок действия";
                    case SubscriptionStatus.NotObtain:
                        return "Не приобретён";
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
                    case "Активный":
                        return SubscriptionStatus.Active;
                    case "Истёк срок действия":
                        return SubscriptionStatus.Expired;
                    case "Не приобретён":
                        return SubscriptionStatus.NotObtain;
                    default:
                        return SubscriptionStatus.NotObtain;
                }
            }
            return SubscriptionStatus.NotObtain;
        }
    }

    public class SubscriptionStatusesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var newList = new List<string>();
            if (value != null && value is List<SubscriptionStatus> statuses)
            {
                foreach (var status in statuses)
                switch (status)
                {
                    case SubscriptionStatus.Active:
                        newList.Add("Активный");
                            break;
                    case SubscriptionStatus.Expired:
                        newList.Add("Истёк срок действия");
                            break;
                    case SubscriptionStatus.NotObtain:
                        newList.Add("Не приобретён");
                            break;
                }
            }
            return newList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var newList = new List<SubscriptionStatus>();
            if (value != null && value is List<string> strs)
            {
                foreach (var str in strs)
                    switch (str)
                    {
                        case "Активный":
                             newList.Add(SubscriptionStatus.Active);
                            break;
                        case "Истёк срок действия":
                            newList.Add(SubscriptionStatus.Expired);
                            break;
                        case "Не приобретён":
                            newList.Add(SubscriptionStatus.NotObtain);
                            break;
                        default:
                            newList.Add(SubscriptionStatus.NotObtain);
                            break;
                    }
            }
            return newList;
        }
    }
}
