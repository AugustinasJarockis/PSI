using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;
using System.Collections.Generic;
using System;

namespace NOTEA.Models.Utilities
{
    public static class SelectionBuilder
    {
        //private static IEnumerable<ConspectModel> whereStatement = null;
        //private static IEnumerable<ConspectModel> orderStatement = null;
        public static Func<IQueryable<ConspectModel>, List<ConspectModel>> Build<OrderReturnType>(Func<ConspectModel, bool> filter = null, Func<ConspectModel, OrderReturnType> order = null, bool orderDescending = false)
        {
            //Func<IQueryable<ConspectModel>, IQueryable<ConspectModel>> t = null;
            //t = c => c;
            //t += t.OrderBy(order);
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
