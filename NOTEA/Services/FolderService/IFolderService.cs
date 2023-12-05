using NOTEA.Models.FileTree;

namespace NOTEA.Services.FolderService
{
    public interface IFolderService
    {
        public int AddFolder(string name);
        public void DeleteFolder(int user_id, int folder_id);
        public string? GetFolderName(int id);
        public List<FolderModel> GetFolderList(int user_id, int folder_id, Func<IQueryable<FolderModel>, List<FolderModel>> selection = null);
        public int GetPreviousFolderID(int user_id, int current_folder_id);
    }
}
