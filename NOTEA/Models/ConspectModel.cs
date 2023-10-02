using System.ComponentModel.DataAnnotations;

namespace NOTEA.Models
{
    public class ConspectModel
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string Date { get; set; }
        public string Name { get; set; }
        public string ConspectText { get; set; }

        public ConspectModel(){
            Date = "";
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
