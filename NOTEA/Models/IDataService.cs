namespace NOTEA.Models
{
    public interface IDataService
    {
        public void SaveConspect(ConspectModel conspect);
        public void SaveFileName(FileNameModel fileNames, string fileName);
        public ConspectModel LoadConspects(string fileName);
        public FileNameListModel LoadFileNames();
    }
}
