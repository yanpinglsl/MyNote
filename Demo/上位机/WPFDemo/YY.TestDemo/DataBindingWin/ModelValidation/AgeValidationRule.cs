using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataBindingWin.ModelValidation
{
    class AgeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null && double.TryParse(value.ToString(), out double age))
            {
                if (age >= 1 && age <= 100)
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "请输入 1 至 100的年龄");
        }
    }
}
