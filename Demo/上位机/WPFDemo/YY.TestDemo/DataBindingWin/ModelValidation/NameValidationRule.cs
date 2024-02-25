using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataBindingWin.ModelValidation
{
    public class NameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null && value.ToString().Length >= 1 && value.ToString().Length <= 10)
                return new ValidationResult(true, "通过");
            else
                return new ValidationResult(false, "用户名长度1-10个字符");
        }
    }
}
