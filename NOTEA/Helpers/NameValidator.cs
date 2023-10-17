namespace NOTEA.Helpers
{
    public static class NameValidator
    {
        public static bool IsValidName(this string name)
        {
            if (name == null || name.Length > 80 || name[0] == '.' || name.Contains('\\') ||
                name.Contains('/') || name.Contains('?') || name.Contains('*') ||
                name.Contains(':') || name.Contains('"') || name.Contains('<') ||
                name.Contains('>') || name.Contains('|'))
                return false;
            return true;
        }
    }
}
