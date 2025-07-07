using System.Linq;

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
    IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;

    /// <summary>
    /// Get a single item by its key
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to retrieve </typeparam>
    /// <param name="key"> The primary key of the item </param>
    /// <returns> The item if found; otherwise null </returns>
    TEntity? GetByID<TEntity>(object key) where TEntity : class;

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to create </typeparam>
    /// <param name="entity"> The item instance to create </param>
    void Create<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Update an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to update </typeparam>
    /// <param name="entity"> The item instance with updated values </param>
    void Update<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Delete an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity to delete </typeparam>
    /// <param name="entity"> The item to remove </param>
    void Delete<TEntity>(TEntity entity) where TEntity : class;
}
