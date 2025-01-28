using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SomwApp.presentation.rules
{
    public class NotEmptyStringRule : ValidationRule
    {

        /// <summary>
        /// Проверка соответствия значения требованиям: значение - строка, строка не пустая.
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="cultureInfo"></param>
        /// <returns>Объект типа ValidationResult</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = string.Empty;
            try
            {
                input = Convert.ToString((string)value);
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, $"Invalid characters or {ex.Message}");
                throw;
            }
            if (string.IsNullOrEmpty(input)) return new ValidationResult(false, "Value cannot be empty or non string type");
            return new ValidationResult(true, null);
        }
    }
}
