### Navigate:
- [DynamicQuery Class](#dynamic-query-implementation)
- [IQueryable Paginate Extensions](#iqueryable-paginate-extensions)
- [Sync and Async Repository](#sync-and-async-repository)
- [IQueryable Dynamic Filter Extensions](#iqueryable-dynamic-filter-extensions)
---

# Dynamic Query Implementation

This project provides a dynamic query system for use in persistence, allowing for flexible and dynamic queries with sorting and filtering capabilities.

## DynamicQuery Class

The `DynamicQuery` class represents a dynamic query with the following properties:

- `IEnumerable<Sort>? Sort`: Represents sorting parameters. It is a collection of `Sort` objects.
- `Filter? Filter`: Represents filtering parameters. It is a single `Filter` object.

### Constructors

- `DynamicQuery()`: Default constructor.
- `DynamicQuery(IEnumerable<Sort>? sort, Filter? filter)`: Constructor that takes sorting and filtering parameters.

## Filter Class

The `Filter` class represents a filter condition for the query with the following properties:

- `string Field`: The field or property to filter on.
- `string? Value`: The value to compare against (nullable).
- `string Operator`: The comparison operator.
- `string? Logic`: Optional logical operator for combining multiple filters.
- `IEnumerable<Filter> Filters`: Represents a collection of nested filters.

### Constructors

- `Filter()`: Default constructor with empty strings.
- `Filter(string field, string @operator)`: Constructor that takes the field and operator as parameters.

## Sort Class

The `Sort` class represents a sorting condition for the query with the following properties:

- `string Field`: The field or property to sort on.
- `string Dir`: The sort direction.

### Constructors

- `Sort()`: Default constructor with empty strings.
- `Sort(string field, string dir)`: Constructor that takes the field and direction as parameters.

## Reasoning for the Implementation

This implementation allows developers to construct dynamic queries at runtime, providing a structured and flexible way to represent sorting and filtering conditions. It is particularly useful when handling dynamic user input for querying databases or data sources in a more abstract and reusable manner.


## Usage

Here's an example of how to use the `DynamicQuery` class:

```csharp
using Core.Persistence.Dynamic;

// Create a dynamic query with sorting and filtering
var sortConditions = new List<Sort>
{
    new Sort("ColumnName1", "asc"),
    new Sort("ColumnName2", "desc")
};

var filterCondition = new Filter("ColumnName3", ">", "value");

var dynamicQuery = new DynamicQuery(sortConditions, filterCondition);
```



# IQueryable Paginate Extensions

This namespace provides extension methods for `IQueryable<T>` to support pagination. Pagination is a common practice in software development to efficiently handle large sets of data.

### Class: IQueryablePaginateExtensions

#### Method: ToPaginateAsync<T>

Asynchronously converts `IQueryable<T>` to `Paginate<T>`. It takes parameters for the page index, page size, and an optional cancellation token. Asynchronous operations are used for counting and retrieving data.

#### Method: ToPaginate<T>

Synchronously converts `IQueryable<T>` to `Paginate<T>`. Similar to the asynchronous version but uses synchronous operations for counting and retrieving data.

### Class: Paginate<T>

Represents a paginated result, including metadata for pagination and a subset of data.

Properties:
- `Size`: Page size.
- `Index`: Current page index.
- `Count`: Total count of items.
- `Pages`: Total number of pages.
- `Items`: List of items for the current page.
- `HasPrevious`: A boolean indicating if there is a previous page.
- `HasNext`: A boolean indicating if there is a next page.

## Usage

1. Include the `Core.Persistence.Paging` namespace in your project.
2. Use the `ToPaginateAsync<T>` or `ToPaginate<T>` extension methods on your `IQueryable<T>` instances.

Example:

```csharp
using Core.Persistence.Paging;

// Assuming you have an IQueryable<T> instance called 'data'
int pageIndex = 0;
int pageSize = 10;

// Asynchronous example
var paginatedResultAsync = await data.ToPaginateAsync(pageIndex, pageSize);

// Synchronous example
var paginatedResultSync = data.ToPaginate(pageIndex, pageSize);
```

# Sync and Async Repository
## IRepository<TEntity, TEntityId>
This is the synchronous (Sync) version of the repository interface.
  1. **Get**: Retrieves a single entity based on a predicate. It allows optional parameters for including related entities, handling soft-deleted entities, and enabling change tracking.

  2. **GetList**: Retrieves a paginated list of entities based on optional parameters such as a predicate, ordering, inclusion of related entities, pagination details, and handling soft-deleted entities.

  3. **GetListByDynamic**: Retrieves a paginated list of entities based on dynamic query parameters. Similar to GetList but accepts a DynamicQuery object for dynamic conditions.

  4. **Any**: Checks if any entity satisfies the given predicate. It also supports parameters for handling soft-deleted entities and enabling change tracking.

  5. **Add**: Adds a new entity to the repository.

  6. **AddRange**: Adds a collection of entities to the repository.

  7. **Update**: Updates an existing entity in the repository.

  8. **UpdateRange**: Updates a collection of existing entities in the repository.

  9. **Delete**: Soft-deletes or permanently deletes an entity from the repository. It has an optional parameter to indicate whether to perform a permanent deletion.

  10. **DeleteRange**: Soft-deletes or permanently deletes a collection of entities from the repository. It also has an optional parameter for permanent deletion.

## IAsyncRepository<TEntity, TEntityId>
This is the asynchronous (Async) version of the repository interface. It includes the same methods as IRepository, but with asynchronous counterparts. The asynchronous methods return `Task<T` or `Task<ICollection<T>>` and include an additional parameter or cancellation tokens. Here's a quick overview of the asynchronous methods:
1. **GetAsync**: Asynchronously retrieves a single entity based on a predicate. It allows optional parameters for including related entities, handling soft-deleted entities, and enabling change tracking.

2. **GetListAsync**: Asynchronously retrieves a paginated list of entities based on optional parameters such as a predicate, ordering, inclusion of related entities, pagination details, and handling soft-deleted entities.

3. **GetListByDynamicAsync**: Asynchronously retrieves a paginated list of entities based on dynamic query parameters. Similar to `GetList` but accepts a `DynamicQuery` object for dynamic conditions.

4. **AnyAsync**: Asynchronously checks if any entity satisfies the given predicate. It also supports parameters for handling soft-deleted entities and enabling change tracking.

5. **AddAsync**: Asynchronously adds a new entity to the repository.

6. **AddRangeAsync**: Asynchronously adds a collection of entities to the repository.

7. **UpdateAsync**: Asynchronously updates an existing entity in the repository.

8. **UpdateRangeAsync**: Asynchronously updates a collection of existing entities in the repository.

9. **DeleteAsync**: Asynchronously soft-deletes or permanently deletes an entity from the repository. It has an optional parameter to indicate whether to perform a permanent deletion.

10. **DeleteRangeAsync**: Asynchronously soft-deletes or permanently deletes a collection of entities from the repository. It also has an optional parameter for permanent deletion.

### Differences
The key difference between the two interfaces lies in their nature of execution:
- `IRepository (Sync):` Methods perform database operations synchronously, meaning they block the execution until the operation completes.
- `IAsyncRepository (Async)`: Methods perform database operations asynchronously, allowing the calling code to continue its execution while waiting for the database operation to complete. This is beneficial for responsiveness in applications, especially in scenarios where database calls may take some time.


# IQueryable Dynamic Filter Extensions

This utility provides extension methods for applying dynamic filters and sorting to `IQueryable` collections using Dynamic LINQ.

## Usage

1. **ToDynamic**: Applies dynamic filters and sorting to an `IQueryable` based on a `DynamicQuery` object.

    ```csharp
    IQueryable<T> result = yourQueryable.ToDynamic(yourDynamicQuery);
    ```

2. **Filter**: Applies dynamic filters to an `IQueryable` based on a `Filter` object.

    ```csharp
    IQueryable<T> result = IQueryableDynamicFilterExtensions.Filter(yourQueryable, yourFilter);
    ```

3. **Sort**: Applies dynamic sorting to an `IQueryable` based on a collection of `Sort` objects.

    ```csharp
    IQueryable<T> result = IQueryableDynamicFilterExtensions.Sort(yourQueryable, yourSortCollection);
    ```

4. **GetAllFilters**: Retrieves all filters from a given `Filter` structure.

    ```csharp
    IList<Filter> filters = IQueryableDynamicFilterExtensions.GetAllFilters(yourFilter);
    ```

5. **Transform**: Transforms a single filter into a Dynamic LINQ expression.

    ```csharp
    string dynamicExpression = IQueryableDynamicFilterExtensions.Transform(yourFilter, yourFiltersList);
    ```

## DynamicQuery, Filter, and Sort Models

The utility relies on the following models:

- **DynamicQuery**: Represents a dynamic query with optional filters and sorting.
- **Filter**: Represents a filter condition with optional nested filters.
- **Sort**: Represents a sorting condition with a field and direction.

## Example

```csharp
// Create your IQueryable collection
IQueryable<EntityType> entities = ...;

// Create a dynamic query with filters and sorting
DynamicQuery dynamicQuery = new DynamicQuery
{
    Filter = new Filter
    {
        Field = "PropertyName",
        Operator = "eq",
        Value = "SomeValue",
        Logic = "and",
        Filters = new List<Filter>
        {
            new Filter
            {
                Field = "AnotherProperty",
                Operator = "gt",
                Value = "42"
            }
        }
    },
    Sort = new List<Sort>
    {
        new Sort
        {
            Field = "PropertyName",
            Dir = "asc"
        }
    }
};

// Apply dynamic filtering and sorting
IQueryable<EntityType> result = entities.ToDynamic(dynamicQuery);
```