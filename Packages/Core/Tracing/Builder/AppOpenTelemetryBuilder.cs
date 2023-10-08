using System.Diagnostics.CodeAnalysis;
using Elyspio.OpenTelemetry.Constants;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Technical.Options;
using Elyspio.OpenTelemetry.Tracing.Elements.Base;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Elyspio.OpenTelemetry.Tracing.Builder;

/// <summary>
///     Builder permettant de configurer OpenTelemetry
/// </summary>
/// <typeparam name="TAssembly">
///     Marker dans l'application depuis lequel les classes implémentant
///     <see cref="ITracingContext" /> vont être recherchées
/// </typeparam>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class AppOpenTelemetryBuilder<TAssembly>
{
	private readonly AppOpenTelemetryBuilderOptions _options;

	/// <param name="options"></param>
	public AppOpenTelemetryBuilder(AppOpenTelemetryBuilderOptions options)
	{
		_options = options;
	}


	/// <summary>
	///     Permet de configurer les options de l'instrumentation ASP.NET Core
	/// </summary>
	public Action<AspNetCoreInstrumentationOptions>? AspNetCoreInstrumentation { get; set; }

	/// <summary>
	///     Permet de configurer les options de l'instrumentation des clients HTTP
	/// </summary>
	public Action<HttpClientInstrumentationOptions>? HttpClientInstrumentation { get; set; }

	public Action<TracerProviderBuilder>? Tracing { get; set; }

	/// <summary>
	///     Chemins à ignorer pour le tracing
	/// </summary>
	/// <example>
	///     Par défaut : /swagger
	/// </example>
	public string[] IgnorePaths { get; set; } =
	{
		"/swagger"
	};


	/// <summary>
	///     Chemins à ignorer pour le tracing
	/// </summary>
	/// <example>
	///     Par défaut : /swagger
	/// </example>
	public string[] Metters { get; set; } = Array.Empty<string>();



	/// <summary>
	///     Active le tracing dans les services de l'application
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public OpenTelemetryBuilder Build(IServiceCollection services)
	{
		var resourceBuilder = ResourceBuilder.CreateEmpty().AddService(_options.Service, serviceVersion: _options.Version);

		var sources = AssemblyHelper.GetClassWithInterface<TAssembly, ITracingContext>().ToArray();

		return services.AddOpenTelemetry()
			.WithTracing(tracing =>
			{
				tracing.SetResourceBuilder(resourceBuilder);

				tracing.AddSource(sources);

				tracing.AddOtlpExporter(o => { o.Endpoint = _options.CollectorUri; });

				tracing.SetErrorStatusOnException();

				tracing.AddHttpClientInstrumentation(o =>
				{
					o.RecordException = true;
					HttpClientInstrumentation?.Invoke(o);
				});

				tracing.AddAspNetCoreInstrumentation(o =>
				{
					o.RecordException = true;
					o.Filter = ctx => IgnorePaths.All(p => !ctx.Request.Path.StartsWithSegments(p));
					o.EnableGrpcAspNetCoreSupport = false;
					o.EnrichWithException = (activity, exception) => { activity.SetTag("exception", exception); };
					o.EnrichWithHttpResponse = (activity, response) => { activity.DisplayName = $"HTTP - {response.HttpContext.Request.Method} - {response.HttpContext.Request.Path}"; };
					AspNetCoreInstrumentation?.Invoke(o);
				});


				Tracing?.Invoke(tracing);
			})
			.WithMetrics(metric =>
			{
				metric.SetResourceBuilder(resourceBuilder);

				metric.AddOtlpExporter(o => { o.Endpoint = _options.CollectorUri; })
					.AddProcessInstrumentation()
					.AddRuntimeInstrumentation()
					.AddHttpClientInstrumentation()
					.AddAspNetCoreInstrumentation();


				metric.AddMeter(MetterConstants.DefaultMetters.Concat(sources).Concat(Metters).ToArray());


				metric.AddView("request-duration",
					new ExplicitBucketHistogramConfiguration
					{
						Boundaries = new[] { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
					});
			});
	}
}