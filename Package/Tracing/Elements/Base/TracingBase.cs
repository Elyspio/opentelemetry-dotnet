using System.Diagnostics;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Microsoft.Extensions.Logging;

namespace Elyspio.OpenTelemetry.Tracing.Elements.Base;

/// <summary>
///     Tracing context for Services and Adapters
/// </summary>
public class TracingBase : ITracingContext
{
	protected readonly ILogger _logger;
	private readonly string _sourceName;


	/// <summary>
	///     Default constructor
	/// </summary>
	/// <param name="logger"></param>
	protected TracingBase(ILogger logger)
	{
		_logger = logger;
		_sourceName = GetType().Name;
		TracingContext.AddSource(_sourceName);
	}


	private ActivitySource ActivitySource => TracingContext.GetActivitySource(_sourceName);


	/// <summary>
	///     Create a logger instance for a specific call
	/// </summary>
	/// <param name="arguments"></param>
	/// <param name="level"></param>
	/// <param name="method">name of the method (auto)</param>
	/// <param name="fullFilePath">filename of the method (auto)</param>
	/// <param name="autoExit">Pass false to handle <see cref="Log.LoggerInstance.Exit" /> yourself</param>
	/// <param name="logOnExit">if the logger will log in Serilog on exit</param>
	/// <returns></returns>
	internal Log.LoggerInstance LogInternal(string arguments, LogLevel level, string method, string fullFilePath, bool autoExit, bool logOnExit)
	{
		method = TracingContext.GetMethodName(method);

		var className = Log.GetClassNameFromFilepath(fullFilePath);

		var activity = ActivitySource.CreateActivity(className, method, arguments);

		return _logger.Enter(arguments, level, activity, method, autoExit, className, logOnExit);
	}
}