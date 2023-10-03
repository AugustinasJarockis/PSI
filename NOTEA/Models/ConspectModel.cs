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
                if (value == null || value.Length > currentDate.Length)
                {
                    _date = currentDate;
                    return;
                }
                string[] providedDateNumbers = value.Split('-');
                string[] currentDateNumbers = currentDate.Split('-');
                if (Int32.Parse(providedDateNumbers[0]) > Int32.Parse(currentDateNumbers[0]) ||
                    (Int32.Parse(providedDateNumbers[0]) == Int32.Parse(currentDateNumbers[0]) && 
                     (Int32.Parse(providedDateNumbers[1]) > Int32.Parse(currentDateNumbers[1]) ||
                      (Int32.Parse(providedDateNumbers[1]) == Int32.Parse(currentDateNumbers[1]) &&
                       Int32.Parse(providedDateNumbers[2]) > Int32.Parse(currentDateNumbers[2])))))
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
