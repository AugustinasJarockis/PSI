using NOTEA.Models.ConspectModels;
using NOTEA.Models.FileTree;

namespace NOTEA.Repositories.GenericRepositories
{//DELETE THIS FILE
    public interface IGenericRepository<ConspectType> where ConspectType : class
    {
//======= Move to API
       
        //public ConspectListModel<ConspectType> LoadConspects(int user_id, Func<IQueryable<ConspectType>, List<ConspectType>> Select = null, int folder_id = 0);
        //public void AssignToFolder(TreeNodeModel treeNodeModel);
        //public void DeleteConspect (int id, int user_id);
//>>>>>>> main
    }
}
