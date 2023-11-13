using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;
namespace NOTEA.Models.Utilities
{
    public static class SelectionBuilder<OrderReturnType>
    {
        //public static Func<ConspectModel, bool> filter = null;
        //public static Func<ConspectModel, OrderReturnType> order = null;

        //public static Func<DbSet<ConspectModel>, List<ConspectModel>> Build(bool orderDescending = false)
        //{
        //    if (filter == null && order == null)
        //    {
        //        return c => c.ToList();
        //    }
        //    if(filter == null)
        //    {
        //        if (orderDescending)
        //            return c => c.OrderByDescending(order).ToList();
        //        return c => c.OrderBy(order).ToList();
        //    }
        //    if (order == null)
        //    {
        //        return c => c.Where(filter).ToList();
        //    }
        //    if (orderDescending)
        //        return c => c.Where(filter).OrderByDescending(order).ToList();
        //    return c => c.Where(filter).OrderBy(order).ToList();
        //}

        //public static Func<DbSet<ConspectModel>, List<ConspectModel>> Build(Func<ConspectModel, bool> temp_filter = null, Func<ConspectModel, OrderReturnType> temp_order = null, bool orderDescending = false)
        //{
        //    if (filter == null && temp_filter == null && order == null && temp_order == null)
        //    {
        //        return c => c.ToList();
        //    }
        //    if (filter == null && temp_filter == null)
        //    {
        //        if (orderDescending)
        //            return c => c.OrderByDescending(temp_order == null ? order : temp_order).ToList();
        //        return c => c.OrderBy(temp_order == null ? order : temp_order).ToList();
        //    }
        //    if (order == null && temp_order == null)
        //    {
        //        return c => c.Where(temp_filter == null ? filter : temp_filter).ToList();
        //    }
        //    if (orderDescending)
        //        return c => c.Where(temp_filter == null ? filter : temp_filter).OrderByDescending(temp_order == null ? order : temp_order).ToList();
        //    return c => c.Where(temp_filter == null ? filter : temp_filter).OrderBy(temp_order == null ? order : temp_order).ToList();
        //}

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
