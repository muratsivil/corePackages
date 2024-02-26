// , IRepository<TEntity, TEntityId>
using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence.Repositories;

// This class is a base implementation for an asynchronous repository using Entity Framework Core.
public class EfRepositoryBase<TEntity, TEntityId, TContext>
    : IAsyncRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId> // TEntity must inherit from Entity<TEntityId>.
    where TContext : DbContext // TContext must be a subclass of DbContext.
{
    protected readonly TContext Context; // The database context.

    // Constructor to initialize the repository with a provided DbContext.
    public EfRepositoryBase(TContext context)
    {
        Context = context;
    }

    // Adds a single entity to the database asynchronously.
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow; // Set the creation date.
        await Context.AddAsync(entity); // Add the entity to the context.
        await Context.SaveChangesAsync(); // Save changes to the database.
        return entity; // Return the added entity.
    }

    // Adds a collection of entities to the database asynchronously.
    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        foreach (TEntity entity in entities)
            entity.CreatedDate = DateTime.UtcNow; // Set the creation date for each entity.
        await Context.AddRangeAsync(entities); // Add the collection of entities to the context.
        await Context.SaveChangesAsync(); // Save changes to the database.
        return entities; // Return the added entities.
    }

    // Checks if any entity satisfies a given predicate asynchronously.
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
            queryable = queryable.AsNoTracking(); // Disable tracking if not required.
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters(); // Include soft-deleted entities if specified.
        if (predicate != null)
            queryable = queryable.Where(predicate); // Apply the given predicate if provided.
        return await queryable.AnyAsync(cancellationToken); // Check if any entity matches the conditions.
    }

    // Deletes a single entity asynchronously, with an option for permanent deletion.
    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entity, permanent); // Set the entity as deleted.
        await Context.SaveChangesAsync(); // Save changes to the database.
        return entity; // Return the deleted entity.
    }

    // Deletes a collection of entities asynchronously, with an option for permanent deletion.
    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entities, permanent); // Set the entities as deleted.
        await Context.SaveChangesAsync(); // Save changes to the database.
        return entities; // Return the deleted entities.
    }

    // Retrieves a single entity based on a predicate asynchronously.
    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
            queryable = queryable.AsNoTracking(); // Disable tracking if not required.
        if (include != null)
            queryable = include(queryable); // Include related entities if specified.
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters(); // Include soft-deleted entities if specified.
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken); // Retrieve the first entity matching the conditions.
    }

    // Retrieves a paginated list of entities based on provided criteria asynchronously.
    public async Task<Paginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
            queryable = queryable.AsNoTracking(); // Disable tracking if not required.
        if (include != null)
            queryable = include(queryable); // Include related entities if specified.
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters(); // Include soft-deleted entities if specified.
        if (predicate != null)
            queryable = queryable.Where(predicate); // Apply the given predicate if provided.
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken); // Order the query and paginate the results.
        return await queryable.ToPaginateAsync(index, size, cancellationToken); // Paginate the results.
    }

    // Retrieves a paginated list of entities based on a dynamic query asynchronously.
    public async Task<Paginate<TEntity>> GetListByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query().ToDynamic(dynamic); // Apply dynamic filtering.
        if (!enableTracking)
            queryable = queryable.AsNoTracking(); // Disable tracking if not required.
        if (include != null)
            queryable = include(queryable); // Include related entities if specified.
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters(); // Include soft-deleted entities if specified.
        if (predicate != null)
            queryable = queryable.Where(predicate); // Apply the given predicate if provided.
        return await queryable.ToPaginateAsync(index, size, cancellationToken); // Paginate the results.
    }

    // Provides the base query for the repository.
    public IQueryable<TEntity> Query() => Context.Set<TEntity>();

    // Updates a single entity asynchronously.
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedDate = DateTime.UtcNow; // Set the update date.
        Context.Update(entity); // Mark the entity as updated.
        await Context.SaveChangesAsync(); // Save changes to the database.
        return entity; // Return the updated entity.
    }

    // Updates a collection of entities asynchronously.
    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
    {
        foreach (TEntity entity in entities)
            entity.UpdatedDate = DateTime.UtcNow; // Set the update date for each entity.
        Context.UpdateRange(entities); // Mark the entities as updated.
        await Context.SaveChangesAsync(); // Save changes to the database.
        return entities; // Return the updated entities.
    }

    // Handles the logic for setting an entity as deleted, either soft or permanent.
    protected async Task SetEntityAsDeletedAsync(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityHaveOneToOneRelation(entity); // Check for one-to-one relationships to avoid issues with soft delete.
            await setEntityAsSoftDeletedAsync(entity); // Set the entity as soft deleted.
        }
        else
        {
            Context.Remove(entity); // Remove the entity permanently.
        }
    }

    // Checks if an entity has a one-to-one relationship and throws an exception if found.
    protected void CheckHasEntityHaveOneToOneRelation(TEntity entity)
    {
        // Logic to check if the entity has a one-to-one relationship.
        bool hasEntityHaveOneToOneRelation =
            Context
                .Entry(entity)
                .Metadata.GetForeignKeys()
                .All(
                    x =>
                        x.DependentToPrincipal?.IsCollection == true
                        || x.PrincipalToDependent?.IsCollection == true
                        || x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()
                ) == false;
        if (hasEntityHaveOneToOneRelation)
            throw new InvalidOperationException(
                "Entity has one-to-one relationship. Soft Delete causes problems if you try to create an entry again by the same foreign key."
            );
    }

    // Sets an entity as soft deleted and cascades the operation to related entities.
    private async Task setEntityAsSoftDeletedAsync(IEntityTimestamps entity)
    {
        if (entity.DeletedDate.HasValue)
            return; // Skip if the entity is already marked as deleted.

        entity.DeletedDate = DateTime.UtcNow; // Set the deletion timestamp.

        var navigations = Context
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
            .ToList();

        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue; // Skip if the navigation target is owned.

            if (navigation.PropertyInfo == null)
                continue; // Skip if the navigation property info is not available.

            object? navValue = navigation.PropertyInfo.GetValue(entity);

            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    // Load related collection if not loaded.
                    IQueryable query = Context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();
                    if (navValue == null)
                        continue; // Skip if the related collection is still null.
                }

                foreach (IEntityTimestamps navValueItem in (IEnumerable)navValue)
                    await setEntityAsSoftDeletedAsync(navValueItem);
            }
            else
            {
                if (navValue == null)
                {
                    // Load related entity if not loaded.
                    IQueryable query = Context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType())
                        .FirstOrDefaultAsync();
                    if (navValue == null)
                        continue; // Skip if the related entity is still null.
                }

                await setEntityAsSoftDeletedAsync((IEntityTimestamps)navValue);
            }
        }

        Context.Update(entity); // Mark the entity as updated after setting it as soft deleted.
    }

    // Retrieves a queryable for related entities, excluding soft-deleted ones.
    protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        Type queryProviderType = query.Provider.GetType();
        MethodInfo createQueryMethod =
            queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
            ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");

        // Create a queryable for the related entities, excluding soft-deleted ones.
        var queryProviderQuery =
            (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: new object[] { query.Expression })!;
        return queryProviderQuery.Where(x => !((IEntityTimestamps)x).DeletedDate.HasValue);
    }

    // Sets a collection of entities as deleted, either soft or permanent.
    protected async Task SetEntityAsDeletedAsync(IEnumerable<TEntity> entities, bool permanent)
    {
        foreach (TEntity entity in entities)
            await SetEntityAsDeletedAsync(entity, permanent); // Set each entity as deleted.
    }

}