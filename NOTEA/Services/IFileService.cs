using NOTEA.Models;

namespace NOTEA.Services
{
    public interface IFileService
    {
        public void SaveConspect<ConspectType>(ConspectType conspect) where ConspectType : IConspectModel;
        public ConspectType LoadConspect<ConspectType>(string filePath);
        public ConspectListModel<ConspectType> LoadConspects<ConspectType>(string directoryPath);
    }
}
