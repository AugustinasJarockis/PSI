namespace NoteaAPI.Models.FileTree
{
    public class FolderModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date {  get; set; } 

        public FolderModel(string name)
        {
            Name = name;
            Date = DateTime.Now;
        }
    }
}
