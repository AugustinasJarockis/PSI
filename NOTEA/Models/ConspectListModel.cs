using System.Collections;

namespace NOTEA.Models
{
    public class ConspectListModel<ConspectType>
    {
        public List<ConspectType> Conspects = new List<ConspectType>();
        public ConspectListModel() { }
        public ConspectListModel(List<ConspectType> conspects) { this.Conspects = conspects; }

    }

}
