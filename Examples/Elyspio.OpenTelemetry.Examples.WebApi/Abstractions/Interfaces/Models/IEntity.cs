using MongoDB.Bson;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Models;

/// <summary>
///     IEntity interface that represents an entity with a unique identifier.
/// </summary>
public interface IEntity
{
	/// <summary>
	///     The ObjectId unique identifier of an entity.
	/// </summary>
	public ObjectId Id { get; set; }
}