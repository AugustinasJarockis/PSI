using NoteaAPI.Database;
using NoteaAPI.Models.FileTree;
using NoteaAPI.Repositories.GenericRepositories;
using NoteaAPI.Models.ConspectModels;

namespace NoteaAPI.Services.FolderService
{
    public class FolderService : IFolderService
    {
        private readonly DatabaseContext _database;
        private readonly IGenericRepository<ConspectModel> _repository;
        public FolderService(DatabaseContext database, IGenericRepository<ConspectModel> repository)
        {
            _database = database;
            _repository = repository;
        }
        public int AddFolder(string name)
        {
            var temp = new FolderModel(name);
            _database.Folders.Add(temp);
            _database.SaveChanges();
            return temp.Id;
        }
        public void DeleteFolder(int user_id, int folder_id)
        {
            foreach (var notea in _database.FileStructure.Where(f => f.NodeType == NodeType.File && f.UserID == user_id && f.FolderID == folder_id).ToList())
            {
                _database.FileStructure.Remove(notea);
                _repository.DeleteConspect(notea.UnderlyingId, notea.UserID);
            }
            foreach (var folder in _database.FileStructure.Where(f => f.NodeType == NodeType.Folder && f.UserID == user_id && f.FolderID == folder_id).ToList())
            {
                DeleteFolder(folder.UserID, folder.UnderlyingId);
            }
            _database.FileStructure.Remove(_database.FileStructure.Where(f => f.UnderlyingId == folder_id && f.UserID == user_id && f.NodeType == NodeType.Folder).First());
            _database.Folders.Remove(_database.Folders.Find(folder_id));
            _database.SaveChanges();
        }
        public string? GetFolderName(int id)
        {
            return _database.Folders.Find(id)?.Name;
        }
        public List<FolderModel> GetFolderList(int user_id, int folder_id, Func<IQueryable<FolderModel>, List<FolderModel>> selection = null)
        {
            return selection == null ? _database.FileStructure.Where(fs => fs.UserID == user_id && fs.FolderID == folder_id && fs.NodeType == NodeType.Folder)
                                          .Join(_database.Folders, fs => fs.UnderlyingId, f => f.Id, (fs, f) => f)
                                          .ToList()
                                     : selection(_database.FileStructure.Where(fs => fs.UserID == user_id && fs.FolderID == folder_id && fs.NodeType == NodeType.Folder)
                                          .Join(_database.Folders, fs => fs.UnderlyingId, f => f.Id, (fs, f) => f));
        }
        public int GetPreviousFolderID(int user_id, int current_folder_id)
        {
            var temp = _database.FileStructure.Where(f => f.UnderlyingId == current_folder_id && f.NodeType == NodeType.Folder).FirstOrDefault();
            if(temp != null)
                return temp.FolderID;
            return current_folder_id;
        }
    }
}
