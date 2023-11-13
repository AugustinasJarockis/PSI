namespace NOTEA.Models.Utilities.Comparers
{
    public class StringAsOpositeNumberComparer : IComparer<string>
    {
        int IComparer<string>.Compare(string x, string y)
        {
            if (x.Length > y.Length)
                return -1;
            if (x.Length < y.Length)
                return 1;
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] > y[i])
                    return -1;
                if (x[i] < y[i])
                    return 1;
            }
            return 0;
        }
    }
}
