using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SomwApp.presentation.rules
{
    public class AgeInputRule : ValidationRule
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
            if (port < 0 || port >= 100) return new ValidationResult(false, "Value cannot be less than zero and greater than 100");
            return new ValidationResult(true, null);
        }
    }
}
