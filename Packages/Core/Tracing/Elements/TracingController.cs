using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elyspio.OpenTelemetry.Tracing.Elements;

/// <inheritdoc cref="ControllerBase" />
/// with tracing context
public class TracingController : ControllerBase, ITracingContext
{
	protected readonly ILogger _logger;
	private readonly string _sourceName;


	/// <inheritdoc />
	protected TracingController(ILogger logger)
	{
		_logger = logger;
		_sourceName = GetType().Name;
		TracingContext.AddSource(_sourceName);
	}

	private ActivitySource ActivitySource => TracingContext.GetActivitySource(_sourceName);


	/// <summary>
	///     Create a <see cref="arguments" /> for this method
	/// </summary>
	/// <param name="fullFilePath"></param>
	/// <param name="autoExit"></param>
	/// <param name="fullFilePath"></param>
	/// <param name="autoExit"></param>
	/// <returns></returns>
	protected Log.LoggerInstance LogController(string arguments = "", [CallerMemberName] string method = "", [CallerFilePath] string fullFilePath = "", bool autoExit = true)
	{
		method = TracingContext.GetMethodName(method);

		var className = Log.GetClassNameFromFilepath(fullFilePath);
		var activity = ActivitySource.CreateActivity(className, method, arguments);

		return _logger.Enter(arguments, LogLevel.Information, activity, method, autoExit, className);
	}
}