using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NOTEA.Models
{
    public static class DateComparer
    {
        public static bool IsLater(this DateTime date, DateTime other_date)
        {
            if (date > other_date)
                return true;
            return false;
        }
    }
}
