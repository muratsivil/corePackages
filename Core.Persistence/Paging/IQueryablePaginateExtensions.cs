using Microsoft.EntityFrameworkCore;

namespace Core.Persistence.Paging
{
    // Extension methods for IQueryable to support pagination
    public static class IQueryablePaginateExtensions
    {
        // Asynchronously convert IQueryable<T> to Paginate<T>
        public static async Task<Paginate<T>> ToPaginateAsync<T>(
            this IQueryable<T> source,
            int index,
            int size,
            CancellationToken cancellationToken = default
        )
        {
            // Get the total count of items in the IQueryable
            int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);

            // Retrieve a paginated subset of items from the IQueryable
            List<T> items = await source.Skip(index * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);

            // Create and return a Paginate<T> object with the paginated data
            Paginate<T> list = new()
            {
                Index = index,
                Count = count,
                Items = items,
                Size = size,
                Pages = (int)Math.Ceiling(count / (double)size) // Calculate the total number of pages
            };
            return list;
        }

        // Convert IQueryable<T> to Paginate<T>
        public static Paginate<T> ToPaginate<T>(this IQueryable<T> source, int index, int size)
        {
            // Get the total count of items in the IQueryable
            int count = source.Count();

            // Retrieve a paginated subset of items from the IQueryable
            var items = source.Skip(index * size).Take(size).ToList();

            // Create and return a Paginate<T> object with the paginated data
            Paginate<T> list = new()
            {
                Index = index,
                Size = size,
                Count = count,
                Items = items,
                Pages = (int)Math.Ceiling(count / (double)size) // Calculate the total number of pages
            };
            return list;
        }
    }
}
