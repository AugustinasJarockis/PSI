namespace NOTEA.Models
{
    public class FileNameModel
    {
        public string Name { get; set; }
        public FileNameModel(string name)
        { Name = name; }
        public FileNameModel() 
        { Name = ""; }
    }
}
