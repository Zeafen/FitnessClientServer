using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SomwApp.presentation.rules
{
    public class LettersLimitRule : ValidationRule
    {
        public int Min { get; set; } = 0;
        public int Max { get; set; } = int.MaxValue;

        /// <summary>
        /// Проверка соответствия значения требованиям: количество символов с строке не превышает максимального и не меньше минимального
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="cultureInfo"></param>
        /// <returns>Объект типа ValidationResult</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (value is string str && str.Length >= Min && str.Length <= Max)
                    return new ValidationResult(true, null);
                return new ValidationResult(false, $"Inappropriate value^ string length must be in range {Min}..{Max}");
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, $"Invalid characters or {ex.Message}");
                throw;
            }
        }
    }
}
