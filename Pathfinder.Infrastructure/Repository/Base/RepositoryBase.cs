using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pathfinder.Infrastructure.Data;

namespace Pathfinder.Infrastructure.Repository.Base
{
    public class RepositoryBase<T, TId> : IRepositoryBase<T, TId> where T : class, IEntityBase<TId>
    {
        public RepositoryBase(PgDbContext context)
        {
            this.context = context;
        }

        protected readonly DbContext context;

        private DbSet<T> _entities;

        protected virtual DbSet<T> Entities
        {
            get
            {
                return _entities ??= context.Set<T>();
            }
        }

        public void Add(T entity)
        {
            Entities.Add(entity);
        }
        
        public void AddRange(IEnumerable<T> entities)
        {
            Entities.AddRange(entities);
        }

        public virtual async Task<T> GetByIdAsync(TId id)
        {
            return await Entities.FindAsync(id).ConfigureAwait(false);
        }

        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            Entities.AddRange(entities);
            return Task.CompletedTask;
        }

        public void Save(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            Entities.Remove(entity);
        }

        public virtual async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await Entities.ToListAsync().ConfigureAwait(false);
        }

        public IQueryable<T> Table => Entities;

        public IQueryable<T> TableNoTracking => Entities.AsNoTracking();

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.Where(predicate).ToListAsync().ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeString = null,
            bool disableTracking = true)
        {
            var query = disableTracking ? TableNoTracking : Table;

            if (!string.IsNullOrWhiteSpace(includeString))
            {
                query = query.Include(includeString);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync().ConfigureAwait(false);
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true)
        {
            var query = disableTracking ? TableNoTracking : Table;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync().ConfigureAwait(false);
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }
    }
}
