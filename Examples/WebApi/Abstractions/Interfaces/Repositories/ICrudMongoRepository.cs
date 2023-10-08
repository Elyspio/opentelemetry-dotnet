using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Models;
using MongoDB.Bson;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Repositories;

/// <summary>
///     Defines a generic CRUD repository for entities that implement IEntity interface.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TBase">type that is used for creating or updating TEntity</typeparam>
public interface ICrudMongoRepository<TEntity, in TBase> where TEntity : IEntity
{
	/// <summary>
	///     Adds a new entity to the repository.
	/// </summary>
	/// <param name="base"></param>
	/// <returns>The TEntity that was added.</returns>
	public Task<TEntity> Add(TBase @base);

	/// <summary>
	///     Fetches all entity in the repository.
	/// </summary>
	/// <returns>A List of all TEntity objects.</returns>
	public Task<List<TEntity>> GetAll();

	/// <summary>
	///     Deletes a entity from the repository.
	/// </summary>
	/// <param name="id">The identifier of the connection.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	public Task Delete(ObjectId id);

	/// <summary>
	///     Fetches a specific entity by id.
	/// </summary>
	/// <param name="id">The identifier of the connection.</param>
	/// <returns>The TEntity with the specified id.</returns>
	public Task<TEntity?> GetById(ObjectId id);
}