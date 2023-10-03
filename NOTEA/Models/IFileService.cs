namespace NOTEA.Models
{
    public interface IFileService
    {
        public void SaveConspect(ConspectModel conspect);
        public ConspectModel LoadConspect(string filePath);
        public ConspectListModel LoadConspects(string directoryPath);
    }
}
