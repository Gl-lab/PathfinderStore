﻿using Pathfinder.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories.Base
{
    public interface IRepositoryBase<T, TId> where T : IEntityBase<TId>
    {
        IQueryable<T> Table { get; }

        IQueryable<T> TableNoTracking { get; }

        Task<T> GetByIdAsync(TId id);

        Task<IReadOnlyList<T>> ListAllAsync();

        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        Task<T> SaveAsync(T entity);

        void Add(T entity);

        Task DeleteAsync(T entity);

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeString = null,
            bool disableTracking = true);

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true);
    }
}
