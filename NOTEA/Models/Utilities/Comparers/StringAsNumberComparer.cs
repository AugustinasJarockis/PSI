namespace NOTEA.Models.Utilities.Comparers
{
    public class StringAsNumberComparer : IComparer<string>
    {
        int IComparer<string>.Compare(string x, string y)
        {
            return x.CompareTo(y) * -1;
        }
    }
}
