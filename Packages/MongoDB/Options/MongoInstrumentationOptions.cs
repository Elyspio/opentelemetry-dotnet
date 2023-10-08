using MongoDB.Driver.Core.Events;

namespace Elyspio.OpenTelemetry.MongoDB.Options;

/// <summary>
///     Options for mongo instrumentation
/// </summary>
public sealed class MongoInstrumentationOptions
{
	/// <summary>
	///     If true, get the mongodb command in activity
	/// </summary>
	public bool CaptureCommandText { get; set; }

	/// <summary>
	///     Filter the commands to capture
	/// </summary>
	/// <returns>false to discard an event</returns>
	public Func<CommandStartedEvent, bool>? ShouldStartActivity { get; set; }
}