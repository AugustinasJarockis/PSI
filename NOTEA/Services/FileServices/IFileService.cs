using NOTEA.Models.ConspectModels;

namespace NOTEA.Services.FileServices
{
    public interface IFileService
    {
        public void SaveConspect(ConspectModel conspect);
        public ConspectModel LoadConspect(int id);
        public ConspectListModel<ConspectModel> LoadConspects(Func<ConspectModel, bool> filter = null, Func<ConspectModel, bool> order = null);
    }
}
