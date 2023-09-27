namespace NOTEA.Models
{
    public interface IDataService
    {
        public void SaveConspect(ConspectModel conspect);
        public ConspectModel LoadConspect(string filePath);
        public ConspectListModel LoadConspects(string directoryPath);
    }
}
