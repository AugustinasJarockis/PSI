using System.ComponentModel.DataAnnotations;

namespace NOTEA.Models
{
    public class ConspectModel : IComparable<ConspectModel>
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
        public ConspectType ConspectType { get; set; }
        public string Name { get; set; }
        public string ConspectText { get; set; }

        public ConspectModel(){
            _date = "";
            Name = "";
            ConspectText = "";
        }
        public ConspectModel(string name, ConspectType conspectType, string conspectText)
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd");
            Name = name;
            ConspectType = conspectType;
            ConspectText = conspectText;
        }
        /*public ConspectModel(string date, string name, string conspectText)
        {
            Date = date;
            Name = name;
            ConspectText = conspectText;
        }*/
        public int CompareTo(ConspectModel other)
        {
            if (Date.IsGreaterValue(other.Date))
                return 1;
            if (other.Date.IsGreaterValue(Date))
                return -1;
            return Name.CompareTo(other.Name);
        }

    }
    public enum ConspectType
    {
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
