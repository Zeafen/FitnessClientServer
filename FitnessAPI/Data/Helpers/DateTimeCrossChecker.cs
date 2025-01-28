namespace FitnessAPI.Data.Helpers
{
    static  class DateTimeCrossChecker
    {
        /// <summary>
        /// Проверяет, пересекаются ли времернные промежутки
        /// </summary>
        /// <param name="dateFrom1">Начало первого промежутка</param>
        /// <param name="dateTo1">Окончание первого промежутка</param>
        /// <param name="dateFrom2">Начало второго промежутка</param>
        /// <param name="dateTo2">Окончание второго промежутка</param>
        /// <returns>True - промежуьки пересекаются, false - не пересекаются</returns>
        public static bool DoDatesCross(DateTime dateFrom1, DateTime dateTo1, DateTime dateFrom2, DateTime dateTo2)
        {
            var haveSameStart = dateFrom1.CompareTo(dateFrom2) == 0;
            var haveSameEnding = dateTo1.CompareTo(dateTo2) == 0;
            var firstEarlier = dateFrom1.CompareTo(dateFrom2) < 0 && dateTo1.CompareTo(dateFrom2) > 0;
            var secondEarlier = dateFrom2.CompareTo(dateFrom1) < 0 && dateTo2.CompareTo(dateFrom1) > 0;

            return haveSameStart || haveSameEnding || firstEarlier || secondEarlier;
        }
        public static bool DoDatesCrossComplex(DateOnly date1, TimeOnly time1, double duration1, DateOnly date2, TimeOnly time2, double duration2)
        {

            var haveSameDate = date1.CompareTo(date2) == 0;
            var haveSameStart = time1.CompareTo(time2) == 0;
            var haveSameEnding = time1.AddHours(duration1).CompareTo(time2.AddHours(duration2)) == 0;
            var firstEarlier = time1.CompareTo(time2) < 0 && time1.AddHours(duration1).CompareTo(time2) > 0;
            var secondEarlier = time2.CompareTo(time1) < 0 && time2.AddHours(duration2).CompareTo(time1) > 0;

            return haveSameStart || haveSameEnding || firstEarlier || secondEarlier;
        }
        public static bool DoDatesCrossComplex(DateOnly date1, TimeOnly time1, decimal duration1, DateOnly date2, TimeOnly time2, decimal duration2)
        {
            var double1 = Convert.ToDouble(duration1);
            var double2 = Convert.ToDouble(duration2);
            var haveSameDate = date1.CompareTo(date2) == 0;
            var haveSameStart = time1.CompareTo(time2) == 0;
            var haveSameEnding = time1.AddHours(double1).CompareTo(time2.AddHours(double2)) == 0;
            var firstEarlier = time1.CompareTo(time2) < 0 && time1.AddHours(double1).CompareTo(time2) > 0;
            var secondEarlier = time2.CompareTo(time1) < 0 && time2.AddHours(double2).CompareTo(time1) > 0;

            return haveSameStart || haveSameEnding || firstEarlier || secondEarlier;
        }
    }
}
