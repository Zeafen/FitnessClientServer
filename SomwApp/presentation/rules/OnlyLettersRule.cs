using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace SomwApp.presentation.rules
{
    public class OnlyLettersRule : ValidationRule
    {
        /// <summary>
        /// Проверка соответствия значения требованиям: значение - строка и все символы в ней - не числа
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="cultureInfo"></param>
        /// <returns>Объект типа ValidationResult</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string nums = "0123456789";
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

            if(input.Any(nums.Contains))
                    return new ValidationResult(false, "Value cannot contain numbers");

            if (string.IsNullOrEmpty(input)) return new ValidationResult(false, "Value cannot be empty or non string type");
            return new ValidationResult(true, null);
        }
    }
    public class OnlyNumbersRule : ValidationRule
    {
        /// <summary>
        /// Проверка соответствия значения требованиям: значение - строка и все символы в ней - числа
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="cultureInfo"></param>
        /// <returns>Объект типа ValidationResult</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string nums = " 0123456789";
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

            if(!input.Any(nums.Contains))
                    return new ValidationResult(false, "Value cannot contain numbers");

            if (string.IsNullOrEmpty(input)) return new ValidationResult(false, "Value cannot be empty or non string type");
            return new ValidationResult(true, null);
        }
    }
}
