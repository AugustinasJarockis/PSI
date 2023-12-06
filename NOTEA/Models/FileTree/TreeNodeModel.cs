using Microsoft.EntityFrameworkCore;

namespace NOTEA.Models.FileTree
{
    [PrimaryKey(nameof(UserID), nameof(UnderlyingId), nameof(NodeType))]
    public class TreeNodeModel
    {
        public int UnderlyingId {  get; set; }
        public NodeType NodeType { get; set; }
        public int FolderID { get; set; }
        public int UserID { get; set; }

        public TreeNodeModel(){}
        public TreeNodeModel(NodeType nodeType, int underlyingId, int userID, int folderID = 0)
        {
            UnderlyingId = underlyingId;
            NodeType = nodeType;
            FolderID = folderID;
            UserID = userID;
        }
    }
}
