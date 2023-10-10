using System.ComponentModel.DataAnnotations;

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
        public ConspectModel(string date, string name, string conspectText, ConspectSemester conspectSemester = ConspectSemester.Unknown)
        {
            Date = date;
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
