using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Extentions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NoteaAPI.Models.FileTree;

namespace NoteaAPI.Utilities.ListManipulation
{
    public class ListManipulator
    {
        [JsonProperty]
        private bool _filterExists = false;
        [JsonProperty]
        private string? searchBy = "name", searchValue = null;
        public string? SearchBy { get { return searchBy; } }
        public string? SearchValue { get { return searchValue; } }
        public bool FilterExists { get { return _filterExists; } }
        [JsonProperty]
        private SortCollumn? currentSortCollumn = null;
        public SortPhase[] SortStatus { get { return collumnOrderValues; } }
        [JsonProperty]
        private SortPhase[] collumnOrderValues = { SortPhase.None, SortPhase.None, SortPhase.None };

        public Func<IQueryable<ConspectModel>, List<ConspectModel>> GetSelection()
        {
            Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> temp = null;
            Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> tempFilter = GenerateFilter(searchBy, searchValue);
            Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> tempSort = GenerateSort(currentSortCollumn);
            if (tempFilter != null && tempSort != null)
            {
                temp = list => tempSort(tempFilter(list));
            }
            else
            {
                temp += tempFilter + tempSort;
            }
            return temp == null ? null : list => temp(list).ToList();
        }
        public Func<IQueryable<FolderModel>, List<FolderModel>> GetFolderSelection()
        {
            Func<IEnumerable<FolderModel>, IEnumerable<FolderModel>> temp = null;
            Func<IEnumerable<FolderModel>, IEnumerable<FolderModel>> tempFilter = GenerateFolderFilter(searchBy, searchValue);
            Func<IEnumerable<FolderModel>, IEnumerable<FolderModel>> tempSort = GenerateFolderSort(currentSortCollumn);
            if (tempFilter != null && tempSort != null)
            {
                temp = list => tempSort(tempFilter(list));
            }
            else
            {
                temp += tempFilter + tempSort;
            }
            return temp == null ? null : list => temp(list).ToList();
        }
        public Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> GenerateFilter(string searchBy, string searchValue)
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
                return list => list.Where(c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower()));
            }
            return null;
        }
        public Func<IEnumerable<FolderModel>, IEnumerable<FolderModel>> GenerateFolderFilter(string searchBy, string searchValue)
        {
            if (searchValue.IsNullOrEmpty())
            {
                return null;
            }
            if (searchBy.ToLower() == "name")
            {
                return list => list.Where(c => c.Name.ToLower().Contains(searchValue.ToLower()));
            }
            return null;
        }
        public Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> GenerateSort(SortCollumn? collumn)
        {
            if (collumn == null)
                return null;
            Func<IEnumerable<ConspectModel>, IEnumerable<ConspectModel>> orderStatement = null;
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
        private Func<IEnumerable<FolderModel>, IEnumerable<FolderModel>> GenerateFolderSort(SortCollumn? collumn)
        {
            if (collumn == null)
                return null;
            Func<IEnumerable<FolderModel>, IEnumerable<FolderModel>> orderStatement = null;
            switch (collumn + 3 * (int)collumnOrderValues[(int)collumn])
            {
                case SortCollumn.Name + 3 * (int)SortPhase.Ascending:
                    orderStatement = list => list.OrderBy(c => c.Name);
                    break;
                case SortCollumn.Name + 3 * (int)SortPhase.Descending:
                    orderStatement = list => list.OrderByDescending(c => c.Name);
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
    }
}