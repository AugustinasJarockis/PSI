using System.Text.RegularExpressions;

namespace NoteaAPI.Extentions
{
    public static class NameValidator
    {
        public static bool IsValidName(this string name)
        {
            if (name == "" || name == null || name.Length > 80 || name[0] == '.' || name.Contains('\\') ||
                name.Contains('/') || name.Contains('?') || name.Contains('*') ||
                name.Contains(':') || name.Contains('"') || name.Contains('<') ||
                name.Contains('>') || name.Contains('|'))
                return false;
            return true;
        }
        public static bool IsValidEmail (this string email)
        {
            if (email == null || email.Length == 0) return false;
            Regex validateEmailRegex = new Regex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
            if (validateEmailRegex.IsMatch(email))
              return true; 
            return false; 
        }

    }
}
