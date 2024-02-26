using System.Linq.Expressions;
using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Persistence.Repositories
{
    // Generic repository interface for asynchronous operations on entities
    public interface IAsyncRepository<TEntity, TEntityId> : IQuery<TEntity>
        where TEntity : Entity<TEntityId> // Assumes entities implement some base class Entity<TEntityId>
    {
        // Get a single entity based on a predicate
        Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> predicate, // Predicate to filter entities
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, // Optional inclusion/join of related entities
            bool withDeleted = false, // Include soft-deleted entities
            bool enableTracking = true, // Enable Entity Framework change tracking
            CancellationToken cancellationToken = default
        );

        // Get a paginated list of entities based on optional parameters
        Task<Paginate<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>>? predicate = null, // Optional predicate to filter entities
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, // Optional order by clause
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, // Optional inclusion/join of related entities
            int index = 0, // Page index
            int size = 10, // Page size
            bool withDeleted = false, // Include soft-deleted entities
            bool enableTracking = true, // Enable Entity Framework change tracking
            CancellationToken cancellationToken = default
        );

        Task<Paginate<TEntity>> GetListByDynamicAsync(
        DynamicQuery dynamic,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
        );

        // Check if any entity satisfies the given predicate/condition
        Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>>? predicate = null, // Optional predicate to check entities
            bool withDeleted = false, // Include soft-deleted entities
            bool enableTracking = true, // Enable Entity Framework change tracking
            CancellationToken cancellationToken = default
        );

        // Add a new entity to the repository
        Task<TEntity> AddAsync(TEntity entity);

        // Add a collection of entities to the repository
        Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities);

        // Update an existing entity in the repository
        Task<TEntity> UpdateAsync(TEntity entity);

        // Update a collection of existing entities in the repository
        Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities);

        // Soft-delete or permanently delete an entity from the repository
        Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false);

        // Soft-delete or permanently delete a collection of entities from the repository
        Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false);
    }
}
