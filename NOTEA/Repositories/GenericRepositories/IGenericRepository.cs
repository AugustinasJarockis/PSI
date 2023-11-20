using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;

namespace NOTEA.Repositories.GenericRepositories
{
    public interface IGenericRepository<ConspectType> where ConspectType : class
    {
        public void SaveConspect(ConspectType conspect, int id);
        public ConspectType LoadConspect(int id);
        public ConspectListModel<ConspectType> LoadConspects(int user_id, Func<IQueryable<ConspectType>, List<ConspectType>> Select = null);
        public void AssignToUser(int conspect_id, int user_id, char access_type = 'a');
        public void DeleteConspect (int id);
    }
}
