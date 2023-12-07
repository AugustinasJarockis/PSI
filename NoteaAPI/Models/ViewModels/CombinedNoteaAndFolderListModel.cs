using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Models.FileTree;

namespace NoteaAPI.Models.ViewModels
{
    public class CombinedNoteaAndFolderListModel
    {
        public List<FolderModel> Folders { get; set; }
        public List<ConspectModel> Noteas { get; set; }

        public CombinedNoteaAndFolderListModel(List<FolderModel> folders, List<ConspectModel> noteas)
        {
            Folders = folders;
            Noteas = noteas;
        }
    }
}
