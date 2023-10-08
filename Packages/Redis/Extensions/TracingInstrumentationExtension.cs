using OpenTelemetry.Instrumentation.StackExchangeRedis;
using OpenTelemetry.Trace;

namespace Elyspio.OpenTelemetry.Redis.Extensions;

/// <summary>
///     Extensions for redis instrumentation
/// </summary>
public static class TracingInstrumentationExtension
{
	/// <summary>
	///     Add redis instrumentation
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="action"></param>
	/// <returns></returns>
	public static TracerProviderBuilder AddAppRedisInstrumentation(this TracerProviderBuilder builder, Action<StackExchangeRedisInstrumentationOptions>? action = null)
	{
		builder.AddRedisInstrumentation(o =>
		{
			o.SetVerboseDatabaseStatements = true;
			o.Enrich = (activity, command) => { activity.DisplayName = $"Redis ({command.Command})"; };
			action?.Invoke(o);
		});
		return builder;
	}
}