using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserManagement.Data;

/// <summary>
/// Defines an interface for generic, asynchronous data access methods.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Asynchronously retrieves all entities of a given type, including related entities as specified.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
    /// <param name="includes">Navigation properties to include in the query results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all entities with the specified includes.</returns>
    Task<List<TEntity>> GetAllIncludingAsync<TEntity>(params Expression<Func<TEntity, object>>[] includes) where TEntity : class;

    /// <summary>
    /// Asynchronously retrieves an entity by its primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
    /// <param name="key">The primary key value of the entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise, null.</returns>
    Task<TEntity?> GetByIDAsync<TEntity>(object key) where TEntity : class;

    /// <summary>
    /// Asynchronously creates a new entity in the data store.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to create.</typeparam>
    /// <param name="entity">The entity instance to add.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    Task CreateAsync<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Asynchronously updates an existing entity in the data store.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to update.</typeparam>
    /// <param name="entity">The entity instance with updated values.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Asynchronously deletes an entity from the data store.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to delete.</typeparam>
    /// <param name="entity">The entity instance to remove.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;

}
