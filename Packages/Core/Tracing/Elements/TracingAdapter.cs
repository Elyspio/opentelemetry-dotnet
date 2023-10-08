using System.Runtime.CompilerServices;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements.Base;
using Microsoft.Extensions.Logging;

namespace Elyspio.OpenTelemetry.Tracing.Elements;

/// <summary>
///     Tracing context for Repository
/// </summary>
public class TracingAdapter : TracingBase
{
	protected TracingAdapter(ILogger logger) : base(logger)
	{
	}

	protected Log.LoggerInstance LogAdapter(string arguments = "", [CallerMemberName] string method = "", [CallerFilePath] string fullFilePath = "", bool autoExit = true, bool logOnExit = false)
	{
		return LogInternal(arguments, LogLevel.Debug, method, fullFilePath, autoExit, logOnExit);
	}
}