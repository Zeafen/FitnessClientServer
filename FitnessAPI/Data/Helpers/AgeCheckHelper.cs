using static System.Net.Mime.MediaTypeNames;

namespace FitnessAPI.Data.Helpers
{
    public static class AgeCheckHelper
    {
        public static bool CheckAge(DateOnly dateBirth, DateOnly dateTo, int MinAge, int MaxAge = 100)
        {
            if (dateTo.Year - dateBirth.Year > MinAge && dateTo.Year - dateBirth.Year <= MaxAge) return true;
            else if (dateTo.Year - dateBirth.Year == MinAge && ((dateTo.Month > dateBirth.Month) || (dateTo.Month == dateBirth.Month && dateTo.Day >= dateBirth.Day)))
                return true;
            return false;
        }
    }
}
