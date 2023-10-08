namespace Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Models;

/// <summary>
///     ITransport interface that represents a transport entity with a unique identifier.
/// </summary>
public interface ITransport
{
	/// <summary>
	///     The global unique identifier (GUID) of a transport entity. It can only be set during object initialization.
	/// </summary>
	public Guid Id { get; init; }
}