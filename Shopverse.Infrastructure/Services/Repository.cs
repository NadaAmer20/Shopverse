using Microsoft.EntityFrameworkCore;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Infrastructure.Services
{
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        protected DbSet<TEntity> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<TEntity>();
        }


        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null, bool splitQuery = false)
        {
            IQueryable<TEntity> query = dbSet.Where(predicate);

            if (include != null)
                query = include(query);

            if (splitQuery)
                query = query.AsSplitQuery();

            return await query.ToListAsync(cancellationToken);
        }
        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRangeAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
        {
            var records = await dbSet.Where(filter).ToListAsync(cancellationToken);
            if (records.Any())
            {
                dbSet.RemoveRange(records);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            if (entities != null && entities.Any())
            {
                dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }


        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            _context.Set<TEntity>().UpdateRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = dbSet.Where(e => !e.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync(cancellationToken);
        }


        public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null, CancellationToken cancellationToken = default, bool spiltQuery = false)
        {
            IQueryable<TEntity> query = dbSet.Where(e => !e.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            if (additionalQuery != null)
                query = additionalQuery(query);

            if (spiltQuery)
                query = query.AsSplitQuery();

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id,
            Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
            CancellationToken cancellationToken = default, bool spiltQuery = false)
        {
            IQueryable<TEntity> query = dbSet.Where(e => !e.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            if (additionalQuery != null)
                query = additionalQuery(query);

            if (spiltQuery)
                query = query.AsSplitQuery();

            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public virtual async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter,
             Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null, CancellationToken cancellationToken = default, bool spiltQuery = false)
        {
            IQueryable<TEntity> query = dbSet.Where(e => !e.IsDeleted)
                                             .Where(filter);

            if (additionalQuery != null)
                query = additionalQuery(query);

            if (spiltQuery)
                query = query.AsSplitQuery();

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = dbSet.Where(e => !e.IsDeleted)
                                             .AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync(cancellationToken);
        }

        public virtual async Task<List<TResult>> SelectListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = dbSet.Where(e => !e.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            if (additionalQuery != null)
                query = additionalQuery(query);

            return await query.Select(selector).ToListAsync(cancellationToken);
        }

        public virtual async Task<TResult?> GetSinglePropertyValueAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = dbSet.Where(e => !e.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
        }
        public virtual async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
            => await dbSet.Where(e => !e.IsDeleted)
                          .AnyAsync(filter, cancellationToken);
        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = false;
            entity.CreatedAt = DateTime.Now.ToUniversalTime();
            entity.UpdatedAt = DateTime.Now.ToUniversalTime();
            await dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await dbSet.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }
        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }
        public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now.ToUniversalTime();
            dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await dbSet.Where(predicate).ToListAsync(cancellationToken);
        }
        public IQueryable<TEntity> GetQueryable(bool execludeDeleted = true, CancellationToken cancellationToken = default)
        {
            if (_context == null)
            {
                throw new ObjectDisposedException(nameof(ApplicationDbContext), "The database context has been disposed.");
            }
            return execludeDeleted ? dbSet.Where(e => !e.IsDeleted) : dbSet.AsQueryable();
        }

    }
}
