using System.ComponentModel.DataAnnotations;

namespace NOTEA.Models
{
    public class ConspectModel
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
        public string Name { get; set; }
        public string ConspectText { get; set; }

        public ConspectModel(){
            _date = "";
            Name = "";
            ConspectText = "";
        }
        public ConspectModel(string name, string conspectText)
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd");
            Name = name;
            ConspectText = conspectText;
        }
        public ConspectModel(string date, string name, string conspectText)
        {
            Date = date;
            Name = name;
            ConspectText = conspectText;
        }
    }
}
