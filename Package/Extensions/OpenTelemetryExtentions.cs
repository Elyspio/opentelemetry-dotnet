using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Elyspio.OpenTelemetry.Extensions;

/// <summary>
///     OpenTelemetry Extensions methods for <see cref="IServiceCollection" />
/// </summary>
public static class OpenTelemetryExtentions
{
	/// <summary>
	///     Activate open telemetry support
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	/// <returns></returns>
	public static IServiceCollection AddAppOpenTelemetry<TMarker>(this IServiceCollection services, IConfiguration configuration)
	{
		var sources = AssemblyHelper.GetClassWithInterface<TMarker, ITracingContext>().ToArray();

		services.AddOptions<OtlpExporterOptions>().Configure(opts => { opts.Endpoint = new Uri(configuration["OpenTelemetry:Url"]!); });

		services.AddOpenTelemetryEventLogging();

		services.AddOpenTelemetry()
			.ConfigureResource(conf => conf.AddService(configuration["OpenTelemetry:Service"]!))
			.WithTracing(tracingBuilder =>
			{
				tracingBuilder
					.SetErrorStatusOnException()
					.AddSource(sources)
					// Configure adapter
					.AddSqlClientInstrumentation(o =>
					{
						o.EnableConnectionLevelAttributes = true;
						o.RecordException = true;
						o.SetDbStatementForText = true;
						o.SetDbStatementForStoredProcedure = true;
					})
					.AddAspNetCoreInstrumentation(o =>
					{
						o.RecordException = true;
						o.Filter = ctx => ctx.Request.Path.StartsWithSegments("/api");
						o.EnableGrpcAspNetCoreSupport = false;
						o.EnrichWithException = (activity, exception) => { activity.SetTag("exception", exception); };
					})
					.AddHttpClientInstrumentation(options => { options.RecordException = true; })
					// Configure exporters
					.AddOtlpExporter();

				if (configuration["AuraAdapterConfiguration:Redis:Host"] is not null)
					tracingBuilder.AddRedisInstrumentation(o =>
					{
						o.SetVerboseDatabaseStatements = true;
						o.Enrich = (activity, command) => { activity.DisplayName = $"Redis ({command.Command})"; };
					});
			}).WithMetrics(metricBuilder =>
			{
				metricBuilder
					.AddMeter("*")
					.AddRuntimeInstrumentation()
					.AddProcessInstrumentation()
					.AddHttpClientInstrumentation()
					.AddAspNetCoreInstrumentation(o => { o.Filter = (_, ctx) => ctx.Request.Path != "/metrics"; })
					.AddPrometheusExporter()
					.AddOtlpExporter();
			});

		return services;
	}
}