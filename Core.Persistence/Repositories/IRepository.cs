using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories
{
    // Generic repository interface for synchronous operations on entities
    public interface IRepository<TEntity, TEntityId> : IQuery<TEntity>
        where TEntity : Entity<TEntityId> // Assumes entities implement some base class Entity<TEntityId>
    {
        // Get a single entity based on a predicate
        TEntity? Get(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool withDeleted = false,
            bool enableTracking = true
        );

        // Get a paginated list of entities based on optional parameters
        Paginate<TEntity> GetList(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int index = 0,
            int size = 10,
            bool withDeleted = false,
            bool enableTracking = true
        );

        // Get a paginated list of entities based on dynamic query parameters
        Paginate<TEntity> GetListByDynamic(
            DynamicQuery dynamic,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int index = 0,
            int size = 10,
            bool withDeleted = false,
            bool enableTracking = true
        );

        // Check if any entity satisfies the given predicate
        bool Any(
            Expression<Func<TEntity, bool>>? predicate = null,
            bool withDeleted = false,
            bool enableTracking = true
        );

        // Add a new entity to the repository
        TEntity Add(TEntity entity);

        // Add a collection of entities to the repository
        ICollection<TEntity> AddRange(ICollection<TEntity> entities);

        // Update an existing entity in the repository
        TEntity Update(TEntity entity);

        // Update a collection of existing entities in the repository
        ICollection<TEntity> UpdateRange(ICollection<TEntity> entities);

        // Soft-delete or permanently delete an entity from the repository
        TEntity Delete(TEntity entity, bool permanent = false);

        // Soft-delete or permanently delete a collection of entities from the repository
        ICollection<TEntity> DeleteRange(ICollection<TEntity> entity, bool permanent = false);
    }
}
