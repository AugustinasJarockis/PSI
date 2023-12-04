using NOTEA.Models.ConspectModels;
using NOTEA.Models.FileTree;

namespace NOTEA.Models.PurelyViewModels
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
