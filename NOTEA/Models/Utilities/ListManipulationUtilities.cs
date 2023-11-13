using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;

namespace NOTEA.Models.Utilities
{
    public static class ListManipulationUtilities
    {
        public static string? searchBy = null;
        public static string? searchValue = null;

        public static SortPhase []collumnOrderValues = { SortPhase.None, SortPhase.None, SortPhase.None};

        public static Func<DbSet<ConspectModel>, List<ConspectModel>>? selection = null;
        public static bool selectionExists = false;
    }
}
