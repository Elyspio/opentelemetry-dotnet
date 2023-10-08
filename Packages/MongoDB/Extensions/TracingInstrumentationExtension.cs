using Elyspio.OpenTelemetry.MongoDB.Business;
using OpenTelemetry.Trace;

namespace Elyspio.OpenTelemetry.MongoDB.Extensions;

/// <summary>
///     Extensions for mongo instrumentation
/// </summary>
public static class TracingInstrumentationExtension
{
	/// <summary>
	///     Add mongo instrumentation
	/// </summary>
	/// <param name="builder"></param>
	/// <returns></returns>
	public static TracerProviderBuilder AddAppMongoInstrumentation(this TracerProviderBuilder builder)
	{
		builder.AddSource(MongoDbActivityEventSubscriber.ActivitySourceName);
		return builder;
	}
}