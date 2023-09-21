﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements.Base;
using Microsoft.Extensions.Logging;

namespace Elyspio.OpenTelemetry.Tracing.Elements;

/// <inheritdoc cref="Attribute" />
/// Add tracing context, used for filters/middlewares, etc.
public abstract class TracingAttribute : Attribute, ITracingContext
{
	private readonly string _sourceName;

	/// <inheritdoc />
	protected TracingAttribute()
	{
		_sourceName = GetType().Name;
		TracingContext.AddSource(_sourceName);
	}

	/// <summary>
	///     A logger
	/// </summary>
	public abstract ILogger Logger { get; set; }

	private ActivitySource ActivitySource => TracingContext.GetActivitySource(_sourceName);


	/// <summary>
	///     Start a new Activity for this context
	/// </summary>
	/// <param name="arguments"></param>
	/// <param name="method"></param>
	/// <param name="fullFilePath"></param>
	/// <param name="autoExit"></param>
	/// <returns></returns>
	protected Log.LoggerInstance LogAttribute(string arguments = "", [CallerMemberName] string method = "", [CallerFilePath] string fullFilePath = "", bool autoExit = true)
	{
		method = TracingContext.GetMethodName(method);

		var className = Log.GetClassNameFromFilepath(fullFilePath);
		var activity = ActivitySource.CreateActivity(className, method, arguments);
		return Logger.Enter(arguments, LogLevel.Debug, activity, method, autoExit, className);
	}
}