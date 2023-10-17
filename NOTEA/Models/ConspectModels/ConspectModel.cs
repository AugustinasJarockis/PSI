using System.ComponentModel.DataAnnotations;
using NOTEA.Models.RecordModels;

namespace NOTEA.Models.ConspectModels
{
    public class ConspectModel : IComparable<ConspectModel>, IConspectModel
    {
        public DateTime Date { get; set; }
        public ConspectSemester ConspectSemester { get; set; }
        public string Name { get; set; }
        public string ConspectText { get; set; }

        public LinkedList<RecordModel> ConspectRecords { get; set; } = new LinkedList<RecordModel>();

        public ConspectModel()
        {
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
            if (Date.CompareTo(other.Date) != 0)
                return Date.CompareTo(other.Date);
            return Name.CompareTo(other.Name);
        }
    }

}
