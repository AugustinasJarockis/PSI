using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NOTEA.Models
{
    public static class StringValueComparer
    {
        public static bool IsGreaterValue(this string str, string other_str)
        {
            if (str.Length > other_str.Length)
                return true;
            if (str.Length < other_str.Length)
                return false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] > other_str[i])
                    return true;
                if (str[i] < other_str[i])
                    return false;
            }
            return false;
        }
    }
}
