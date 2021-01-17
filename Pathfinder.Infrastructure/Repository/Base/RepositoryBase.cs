using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Repositories.Base;
using Pathfinder.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public async virtual Task<T> GetByIdAsync(TId id)
        {
            return await Entities.FindAsync(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            Entities.AddRange(entities);
            await context.SaveChangesAsync().ConfigureAwait(false);

            return entities;
        }

        public async Task<T> SaveAsync(T entity)
        {
            if (entity.Id == null || entity.Id.Equals(default(TId)))
            {
                Entities.Add(entity);
            }
            else
            {
                context.Entry(entity).State = EntityState.Modified;
            }

            await context.SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            Entities.Remove(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async virtual Task<IReadOnlyList<T>> ListAllAsync()
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
