using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;
namespace NOTEA.Models.Utilities
{
    public static class SelectionBuilder<OrderReturnType>
    {
        public static Func<DbSet<ConspectModel>, List<ConspectModel>> Build(Func<ConspectModel, bool> filter = null, Func<ConspectModel, OrderReturnType> order = null, bool orderDescending = false)
        {
            if (filter == null && order == null)
            {
                return c => c.ToList();
            }
            if (filter == null)
            {
                if (orderDescending)
                    return c => c.OrderByDescending(order).ToList();
                return c => c.OrderBy(order).ToList();
            }
            if (order == null)
            {
                return c => c.Where(filter).ToList();
            }
            if (orderDescending)
                return c => c.Where(filter).OrderByDescending(order).ToList();
            return c => c.Where(filter).OrderBy(order).ToList();
        }
    }
}
