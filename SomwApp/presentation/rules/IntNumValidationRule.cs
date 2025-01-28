using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SomwApp.presentation.rules
{
    public class IntNumValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int port = 0;
            try
            {
                port = int.Parse((string)value);
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, $"Invalid characters or {ex.Message}");
                throw;
            }
            if (port < 0 || port >= int.MaxValue) return new ValidationResult(false, "Value cannot be less than zero and greater than max accessible value");
            return new ValidationResult(true, null);
        }
    }
    public class DoubleNumValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double port = 0;
            try
            {
                port = double.Parse((string)value);
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, $"Invalid characters or {ex.Message}");
                throw;
            }
            if (port < 0 || port >= double.MaxValue) return new ValidationResult(false, "Value cannot be less than zero and greater than max accessible value");
            return new ValidationResult(true, null);
        }
    }
}
