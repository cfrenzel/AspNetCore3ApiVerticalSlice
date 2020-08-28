using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.EntityFrameworkCore
{
    public static class Extensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int? pageNumber = 1, int? pageSize = 15)
        {
            var skip = (pageNumber.Value - 1) * pageSize.Value;
            return queryable.Skip(skip).Take(pageSize.Value);
        }
    }
}