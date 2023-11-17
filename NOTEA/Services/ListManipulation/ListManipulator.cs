using NOTEA.Models.Utilities;
using NOTEA.Models.ConspectModels;
using NOTEA.Extentions;
using Microsoft.IdentityModel.Tokens;

namespace NOTEA.Services.ListManipulation
{
    public class ListManipulator : IListManipulationService
    {
        private bool _filterExists = false;
        private string? searchBy = null, searchValue = null;
        public bool FilterExists { get { return _filterExists; } }
        private SortCollumn? currentSortCollumn = null;
        public SortPhase[] SortStatus { get { return collumnOrderValues; } }

        //private Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> whereStatement = null;
        //private Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> orderStatement = null;

        public static SortPhase[] collumnOrderValues = { SortPhase.None, SortPhase.None, SortPhase.None };
        public Func<IQueryable<ConspectModel>, List<ConspectModel>> GetSelection()
        {
            Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> temp = null;
            temp += GenerateFilter(searchBy, searchValue) + GenerateSort(currentSortCollumn);
            return temp == null ? null : list => temp(list).ToList();
        }

        private Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> GenerateFilter(string searchBy, string searchValue)
        {
            if (searchValue.IsNullOrEmpty())
            {
                return null;
            }
            if (searchBy.ToLower() == "name")
            {
                return list => list.Where(c => c.Name.ToLower().Contains(searchValue.ToLower()));
            }
            else if (searchBy.ToLower() == "conspectsemester")
            {
                return list => list.Where((Func<ConspectModel, bool>)(c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower())));
            }
            return null;
        }
        public void UpdateFilter(string searchBy, string searchValue)
        {
            if (searchBy.IsNullOrEmpty())
            {
                _filterExists = false;
                this.searchBy = searchBy;
                this.searchValue = searchValue;
                return;
            }
            _filterExists = true;
        }

        private Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> GenerateSort(SortCollumn? collumn)
        {
            if (collumn == null)
                return null;
            Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>>  orderStatement = null;
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
            return orderStatement;
        }
        public void UpdateSort(SortCollumn collumn)
        {
            currentSortCollumn = collumn;

            collumnOrderValues[(int)collumn]++;
            if ((int)collumnOrderValues[(int)collumn] == 3)
                collumnOrderValues[(int)collumn] = SortPhase.None;

            collumnOrderValues[((int)collumn + 1) % 3] = SortPhase.None;
            collumnOrderValues[((int)collumn + 2) % 3] = SortPhase.None;
        }

        public void ClearFilter()
        {
            _filterExists = false;
            searchBy = null;
            searchValue = null;
        }

        public void ClearSort()
        {
            collumnOrderValues[0] = SortPhase.None;
            collumnOrderValues[1] = SortPhase.None;
            collumnOrderValues[2] = SortPhase.None;
            currentSortCollumn = null;
        }

        //public void AddItselfToDict(long userId)
        //{
        //    bool AddSuccesful;
        //    do
        //    {
        //        AddSuccesful = true;
        //        if (!ListManipulatorDictionary.ContainsKey(userId))
        //        {
        //            AddSuccesful = ListManipulatorDictionary.TryAdd(userId, this);
        //        }
        //    } while (!AddSuccesful);
        //}

        //public static void RemoveUserManipulator(long userId)
        //{
        //    bool RemoveSuccesful;
        //    IListManipulationService temp;
        //    do
        //    {
        //        RemoveSuccesful = true;
        //        if (ListManipulatorDictionary.ContainsKey(userId))
        //        {
        //            RemoveSuccesful = ListManipulatorDictionary.TryRemove(userId, out temp);
        //        }
        //    } while (!RemoveSuccesful);
        //}
    }
}
