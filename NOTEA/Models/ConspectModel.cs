namespace NOTEA.Models
{
    public class ConspectModel : IComparable<ConspectModel>, IConspectModel
    {
        public DateTime Date { get; set; }
        public ConspectSemester ConspectSemester { get; set; }
        public string Name { get; set; }
        public string ConspectText { get; set; }

        public ConspectModel(){
            Date = DateTime.Now;
            Name = "";
            ConspectText = "";
        }
        public ConspectModel(string name, string conspectText, ConspectSemester conspectSemester = ConspectSemester.Unknown)
        {
            Date = DateTime.Now;
            Name = name;
            ConspectSemester = conspectSemester;
            ConspectText = conspectText;
        }
        public ConspectModel(DateTime date, string name, string conspectText, ConspectSemester conspectSemester = ConspectSemester.Unknown)
        {
            Date = date;
            Name = name;
            ConspectSemester = conspectSemester;
            ConspectText = conspectText;
        }
        public int CompareTo(ConspectModel other)
        {
            if (Date.IsLater(other.Date))
                return 1;
            if (other.Date.IsLater(Date))
                return -1;
            return Name.CompareTo(other.Name);
        }
    }
    public enum ConspectSemester
    {
        Unknown,
        semester1,
        semester2,
        semester3,
        semester4,
        semester5,
        semester6,
        semester7,
        semester8
    }
}
