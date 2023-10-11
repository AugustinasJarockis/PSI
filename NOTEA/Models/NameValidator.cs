namespace NOTEA.Models
{
    public static class NameValidator
    {
        public static bool IsValidFilename(this string name)
        {
            if(name == null || name.Length > 80 || name[0] == '.' || name.Contains('\\') || name.Contains('/'))
                return false;
            return true;
        }
    }
}
