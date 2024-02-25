using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DataBindingWin.Convert
{
    public class AgeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush background = Brushes.Black;
            if (value != null && double.TryParse(value.ToString(), out double age))
            {
                if (age < 20)
                {
                    background = Brushes.Pink;
                }
                else if (age < 40)
                {
                    background = Brushes.LightGreen;
                }
                else if (age < 60)
                {
                    background = Brushes.LightBlue;
                }
                else if (age < 80)
                {
                    background = Brushes.Orange;
                }
                else if (age < 100)
                {
                    background = Brushes.Gray;
                }
            }
            return background;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
