using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserManagement.Data;

/// <summary>
/// Defines an interface for generic data access methods
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Get a list of items
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to retrieve </typeparam>
    /// <returns> An IQueryable list representing all items of the entity type </returns>
    Task<List<TEntity>> GetAllIncludingAsync<TEntity>(params Expression<Func<TEntity, object>>[] includes) where TEntity : class;

    /// <summary>
    /// Get a single item by its key
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to retrieve </typeparam>
    /// <param name="key"> The primary key of the item </param>
    /// <returns> The item if found; otherwise null </returns>
    Task<TEntity?> GetByIDAsync<TEntity>(object key) where TEntity : class;

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to create </typeparam>
    /// <param name="entity"> The item instance to create </param>
    Task CreateAsync<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Update an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to update </typeparam>
    /// <param name="entity"> The item instance with updated values </param>
    Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Delete an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to delete </typeparam>
    /// <param name="entity"> The item to remove </param>
    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;

}
