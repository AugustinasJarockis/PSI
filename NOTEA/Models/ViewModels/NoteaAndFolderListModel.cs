using NOTEA.Models.ConspectModels;
using NOTEA.Models.FileTree;

namespace NOTEA.Models.ViewModels
{
    public class NoteaAndFolderListModel
    {
        public List<FolderModel> Folders { get; set; }
        public List<ConspectModel> Noteas { get; set; }

        public NoteaAndFolderListModel(List<FolderModel> folders, List<ConspectModel> noteas)
        {
            Folders = folders;
            Noteas = noteas;
        }
    }
}
