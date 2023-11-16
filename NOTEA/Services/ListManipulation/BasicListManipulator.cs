using NOTEA.Models.Utilities;
using NOTEA.Models.ConspectModels;
using NOTEA.Extentions;
using Microsoft.IdentityModel.Tokens;

namespace NOTEA.Services.ListManipulation
{
    public class BasicListManipulator : IListManipulationService
    {
        private bool _filterExists = false;
        public bool FilterExists { get { return _filterExists; } }

        private Func<IQueryable<ConspectModel>, IQueryable<ConspectModel>> whereStatement = null;
        private Func<IQueryable<ConspectModel>, IQueryable<ConspectModel>> orderStatement = null;

        public static SortPhase[] collumnOrderValues = { SortPhase.None, SortPhase.None, SortPhase.None };
        public Func<IQueryable<ConspectModel>, List<ConspectModel>> GetSelection()
        {
            Func<IQueryable<ConspectModel>, IQueryable<ConspectModel>> temp = null;
            temp += whereStatement + orderStatement;
            return temp == null ? null : list => temp(list).ToList();
        }

        public void UpdateFilter(string searchBy, string searchValue)
        {
            if (searchValue.IsNullOrEmpty())
            {
                whereStatement = null;
                _filterExists = false;
                return;
            }
            if (searchBy.ToLower() == "name")
            {
               whereStatement = list => list.Where(c => c.Name.ToLower().Contains(searchValue.ToLower()));
            }
            else if (searchBy.ToLower() == "conspectsemester")
            {
               whereStatement = list => (IQueryable<ConspectModel>)list.Where((Func<ConspectModel, bool>)(c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower())));
            }
            _filterExists = true;
        }

        public void UpdateSort(SortCollumn collumn)
        {
            collumnOrderValues[(int)collumn]++;
            if ((int)collumnOrderValues[(int)collumn] == 3)
                collumnOrderValues[(int)collumn] = SortPhase.None;

            collumnOrderValues[((int)collumn + 1) % 3] = SortPhase.None;
            collumnOrderValues[((int)collumn + 2) % 3] = SortPhase.None;

            orderStatement = null;
            switch (collumn + 3 * (int)collumnOrderValues[(int)collumn])
            {
                case SortCollumn.Name + 3 * (int)SortPhase.Ascending:
                    orderStatement = list => list.OrderBy(c => c.Name);
                    break;
                case SortCollumn.Name + 3 * (int)SortPhase.Descending:
                    orderStatement = list => list.OrderByDescending(c => c.Name);
                    break;
                case SortCollumn.Semester + 3 * (int)SortPhase.Ascending:
                    orderStatement = list => list.OrderBy(c => (int)c.ConspectSemester);
                    break;
                case SortCollumn.Semester + 3 * (int)SortPhase.Descending:
                    orderStatement = list => list.OrderByDescending(c => (int)c.ConspectSemester);
                    break;
                case SortCollumn.Date + 3 * (int)SortPhase.Ascending:
                    orderStatement = list => list.OrderBy(c => c.Date);
                    break;
                case SortCollumn.Date + 3 * (int)SortPhase.Descending:
                    orderStatement = list => list.OrderByDescending(c => c.Date);
                    break;
            }
        }

        public void ClearFilter()
        {
            _filterExists = false;
            whereStatement = null;
        }

        public void ClearSort()
        {
            collumnOrderValues[0] = SortPhase.None;
            collumnOrderValues[1] = SortPhase.None;
            collumnOrderValues[2] = SortPhase.None;
            orderStatement = null;
        }
    }
}
