namespace Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Models;

/// <summary>
///     IEntity interface that represents an entity with a unique identifier.
/// </summary>
public interface ISqlEntity
{
	/// <summary>
	///     The ObjectId unique identifier of an entity.
	/// </summary>
	public Guid Id { get; set; }
}