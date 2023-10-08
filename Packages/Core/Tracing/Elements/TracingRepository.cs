using System.Runtime.CompilerServices;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements.Base;
using Microsoft.Extensions.Logging;

namespace Elyspio.OpenTelemetry.Tracing.Elements;

/// <summary>
///     Tracing context for Repository
/// </summary>
public class TracingRepository : TracingBase
{
	protected TracingRepository(ILogger logger) : base(logger)
	{
	}

	protected Log.LoggerInstance LogRepository(string arguments = "", [CallerMemberName] string method = "", [CallerFilePath] string fullFilePath = "", bool autoExit = true, bool logOnExit = false)
	{
		return LogInternal(arguments, LogLevel.Debug, method, fullFilePath, autoExit, logOnExit);
	}
}