using System.Collections;

namespace NOTEA.Models
{
    public class ConspectListModel<ConspectType>
    {
        public List<ConspectType> conspects = new List<ConspectType>();
        public ConspectListModel() { }
        public ConspectListModel(List<ConspectType> conspects) { this.conspects = conspects; }

    }

}
