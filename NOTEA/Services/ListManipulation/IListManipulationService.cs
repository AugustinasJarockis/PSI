using NOTEA.Models.ConspectModels;
using NOTEA.Models.Utilities;

namespace NOTEA.Services.ListManipulation
{
    public interface IListManipulationService
    {
        public void UpdateFilter(string searchBy, string searchValue);
        public void UpdateSort(SortCollumn collumn);
        public void ClearFilter();
        public void ClearSort();
        public Func<IQueryable<ConspectModel>, List<ConspectModel>> GetSelection();
    }
}
