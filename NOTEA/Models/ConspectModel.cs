using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NOTEA.Models
{
    public class ConspectModel : IComparable<ConspectModel>, IConspectModel
    {

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        private string _date;
        //TODO: Handle exceptions thrown by incorrect format
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string Date {
            get { return _date; }
            set
            {
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                if (value == null || value.IsGreaterValue(currentDate))
                {
                    _date = currentDate;
                    return;
                }
                _date = value;
            }
        }
        public ConspectSemester ConspectSemester { get; set; }
        public string Name { get; set; }
        public string ConspectText { get; set; }

        public LinkedList<RecordModel> ConspectRecords { get; set; } = new LinkedList<RecordModel>();

        public ConspectModel(){
            _date = "";
            Name = "";
            ConspectText = "";
        }
        public ConspectModel(string name, string conspectText, ConspectSemester conspectSemester = ConspectSemester.Unknown)
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd");
            Name = name;
            ConspectSemester = conspectSemester;
            ConspectText = conspectText;
        }
        public int CompareTo(ConspectModel other)
        {
            if (Date.IsGreaterValue(other.Date))
                return 1;
            if (other.Date.IsGreaterValue(Date))
                return -1;
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
