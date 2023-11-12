using NOTEA.Models.ConspectModels;

namespace NOTEA.Services.FileServices
{
    public interface IFileService
    {
        public void SaveConspect(ConspectModel conspect);
        public ConspectModel LoadConspect(int id);
        public ConspectListModel<ConspectModel> LoadConspects();
        public void AssignToUser(int conspect_id, int user_id, char access_type = 'a');
    }
}
