using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Provider;
using System.Windows.Data;
using System.Windows.Media;

namespace DataBindingWin.Convert
{
    public class MultiColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2)
            {
                if (double.TryParse(values[0].ToString(),out double age)
                    && double.TryParse(values[1].ToString(), out double money))
                {
                    if (age < 30 && money > 50000)
                    {
                        return "年纪轻轻的有钱人";
                    }
                    else if (age >= 30 && age <= 60 && money < 5000)
                    {
                        return "悲催的中年人";
                    }
                    else if (age < 30 && money < 5000)
                    {
                        return "这个年轻人没什么钱";
                    }
                    else if (age >= 30 && money > 90000)
                    {
                        return "富豪";
                    }
                    else
                    {
                        return "一个平凡的人";
                    }
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
