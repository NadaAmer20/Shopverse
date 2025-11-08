using Shopverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Abstractions
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null, bool splitQuery = false);
        Task DeleteRangeAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null,
                                         Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
                                         CancellationToken cancellationToken = default, bool spiltQuery = false);

        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter,
                                      Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
                                      CancellationToken cancellationToken = default,
                                      bool spiltQuery = false);

        Task<TEntity?> GetByIdAsync(Guid id, Expression<Func<TEntity, bool>>? filter = null,
                                    Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
                                    CancellationToken cancellationToken = default, bool spiltQuery = false);

        Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null,
                                CancellationToken cancellationToken = default);

        Task<List<TResult>> SelectListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                     Expression<Func<TEntity, bool>>? filter = null,
                                                     Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
                                                     CancellationToken cancellationToken = default);

        Task<TResult?> GetSinglePropertyValueAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                            Expression<Func<TEntity, bool>>? filter = null,
                                                            CancellationToken cancellationToken = default);

        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> filter,
                                CancellationToken cancellationToken = default);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        IQueryable<TEntity> GetQueryable(bool execludeDeleted = true, CancellationToken cancellationToken = default);
    }

}
