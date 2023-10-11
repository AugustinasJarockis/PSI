using System.ComponentModel.DataAnnotations;

namespace NOTEA.Models
{
    public class ConspectModel : IComparable<ConspectModel>, IConspectModel
    {
        public DateTime Date { get; set; }
        public ConspectSemester ConspectSemester { get; set; }
        public string Name { get; set; }
        public string ConspectText { get; set; }

        public LinkedList<RecordModel> ConspectRecords { get; set; } = new LinkedList<RecordModel>();

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
            if (Date.CompareTo(other.Date) != 0)
                return Date.CompareTo(other.Date);
            return Name.CompareTo(other.Name);
        }
    }
    public enum ConspectSemester
    {
        [Display(Name = "Unknown")] Unknown = 0,
        [Display(Name = "2022 autumn")] Autumn_2022 = 1,
        [Display(Name = "2023 spring")] Spring_2023 = 2,
        [Display(Name = "2023 autumn")] Autumn_2023 = 3,
        [Display(Name = "2024 spring")] Spring_2024 = 4,
        [Display(Name = "2024 autumn")] Autumn_2024 = 5,


    }

}
