using System.Collections;

namespace NOTEA.Models
{
    public class ConspectListModel<ConspectType, ConspectSemesterType>
    {
        public List<ConspectType> Conspects = new List<ConspectType>();
        public List<ConspectSemesterType> ConspectSemesters = new List<ConspectSemesterType>();
        public ConspectListModel() { }
        public ConspectListModel(List<ConspectType> conspects, List<ConspectSemesterType> semesters) { this.ConspectSemesters = semesters; this.Conspects = conspects; }

    }

}
