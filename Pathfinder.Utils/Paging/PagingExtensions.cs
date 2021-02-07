using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Pathfinder.Utils.Paging
{
    public static class PagingExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, List<Tuple<SortingOption, Expression<Func<T, object>>>> orderByList)
        {
            if (orderByList == null)
                return query;

            orderByList = orderByList.OrderBy(ob => ob.Item1.Priority).ToList();

            IOrderedQueryable<T> orderedQuery = null;
            foreach (var (item1, item2) in orderByList)
            {
                if (orderedQuery == null)
                {
                    orderedQuery = item1.Direction == SortingOption.SortingDirection.ASC ? query.OrderBy(item2) : query.OrderByDescending(item2);
                }
                else
                {
                    orderedQuery = item1.Direction == SortingOption.SortingDirection.ASC ? orderedQuery.ThenBy(item2) : orderedQuery.ThenByDescending(item2);
                }
            }

            return orderedQuery ?? query;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, List<Tuple<FilteringOption, Expression<Func<T, bool>>>> filterList)
        {
            return filterList == null ? query : filterList.Aggregate(query, (current, filter) => current.Where(filter.Item2));
        }
    }
}
