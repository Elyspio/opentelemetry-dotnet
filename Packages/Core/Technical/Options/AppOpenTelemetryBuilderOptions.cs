namespace Elyspio.OpenTelemetry.Technical.Options;

/// <summary>
///     Options for OpenTelemetryBuilder
/// </summary>
public class AppOpenTelemetryBuilderOptions
{
	/// <summary>
	///
	/// </summary>
	/// <param name="collectorUri"><see cref="CollectorUri"/></param>
	/// <param name="service"><see cref="Service"/></param>
	/// <param name="version"><see cref="Version"/></param>
	public AppOpenTelemetryBuilderOptions(string collectorUri, string service, string? version = null)
	{
		CollectorUri = new Uri(collectorUri);
		Service = service;
		Version = version;
	}

	/// <summary>
	/// Address of the OpenTelemetry collector
	/// </summary>
	public Uri CollectorUri { get; set; }

	/// <summary>
	///     Service name
	/// </summary>
	public string Service { get; set; }

	/// <summary>
	///     Service version
	/// </summary>
	public string? Version { get; set; }
}