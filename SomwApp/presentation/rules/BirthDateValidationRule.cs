using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SomwApp.presentation.rules
{
    public class BirthDateValidationRule : ValidationRule
    {
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateOnly? date = null;
            try
            {
                date = DateOnly.Parse((string)value);
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, $"Invalid characters or {ex.Message}");
                throw;
            }
            if (date.HasValue && CheckAge(date.Value, DateOnly.FromDateTime(DateTime.Now)))  return new ValidationResult(false, "Age must have value between 18 and 100");
            return new ValidationResult(true, null);
        }


        private bool CheckAge(DateOnly dateBirth, DateOnly dateTo)
        {
            if (dateTo.Year - dateBirth.Year > MinAge && dateTo.Year - dateBirth.Year <= MaxAge) return true;
            else if (dateTo.Year - dateBirth.Year == MinAge && ((dateTo.Month > dateBirth.Month) || (dateTo.Month == dateBirth.Month && dateTo.Day >= dateBirth.Day)))
                return true;
            return false;
        }
    }
}
