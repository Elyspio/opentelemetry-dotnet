using Elyspio.OpenTelemetry.Constants;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements.Base;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Instrumentation.StackExchangeRedis;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Elyspio.OpenTelemetry.Tracing.Builder;

/// <summary>
/// Builder permettant de configurer OpenTelemetry
/// </summary>
/// <typeparam name="TAssembly">Marker dans l'application depuis lequel les classes implémentant <see cref="ITracingContext"/> vont être recherchées</typeparam>
public class AppOpenTelemetryBuilder<TAssembly>
{
	private readonly string _serviceName;
	private readonly bool _traceMongo;
	private readonly bool _traceRedis;
	private readonly bool _traceSql;


	/// <param name="serviceName">Le nom du service. Par exemple aura-int-monsisra</param>
	/// <param name="traceSql">Si les traces vers SQL doivent être gérées</param>
	/// <param name="traceMongo">Si les traces vers MongoDB doivent être gérées</param>
	/// <param name="traceRedis">Si les traces vers Redis doivent être gérées</param>
	public AppOpenTelemetryBuilder(string serviceName, bool traceSql = false, bool traceMongo = false, bool traceRedis = false)
	{
		_serviceName = serviceName;
		_traceSql = traceSql;
		_traceMongo = traceMongo;
		_traceRedis = traceRedis;
	}


	/// <summary>
	/// Permet de configurer les options de l'instrumentation SQL
	/// </summary>
	public Action<SqlClientInstrumentationOptions>? SqlInstrumentation { get; set; }

	/// <summary>
	/// Permet de configurer les options de l'instrumentation ASP.NET Core
	/// </summary>
	public Action<AspNetCoreInstrumentationOptions>? AspNetCoreInstrumentation { get; set; }

	/// <summary>
	/// Permet de configurer les options de l'instrumentation des clients HTTP
	/// </summary>
	public Action<HttpClientInstrumentationOptions>? HttpClientInstrumentation { get; set; }

	/// <summary>
	/// Permet de configurer les options de l'instrumentation Redis
	/// </summary>
	public Action<StackExchangeRedisInstrumentationOptions>? RedisInstrumentation { get; set; }

	/// <summary>
	/// Chemins à ignorer pour le tracing
	/// </summary>
	/// <example>
	/// Par défaut : /swagger
	/// </example>
	public  string[] IgnorePaths { get; set; } = new[]
	{
		"/swagger"
	};


	/// <summary>
	/// Chemins à ignorer pour le tracing
	/// </summary>
	/// <example>
	/// Par défaut : /swagger
	/// </example>
	public string[] Metters { get; set; } = Array.Empty<string>();

	/// <summary>
	/// Uri du collector OpenTelemetry
	/// </summary>
	public required Uri OtCollectorUri { get; init; }




	/// <summary>
	/// Active le tracing dans les services de l'application
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public OpenTelemetryBuilder Build(IServiceCollection services)
	{
		var sources = AssemblyHelper.GetClassWithInterface<TAssembly, ITracingContext>().ToArray();

		return services.AddOpenTelemetry()
			.ConfigureResource(conf => conf.AddService(_serviceName))
			.WithTracing(tracing =>
			{
				tracing.AddSource(sources);

				tracing.AddOtlpExporter(o => { o.Endpoint = OtCollectorUri; });

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
					AspNetCoreInstrumentation?.Invoke(o);
				});

				if (_traceSql)
					tracing.AddSqlClientInstrumentation(o =>
					{
						o.EnableConnectionLevelAttributes = true;
						o.RecordException = true;
						o.SetDbStatementForText = true;
						o.SetDbStatementForStoredProcedure = true;
						SqlInstrumentation?.Invoke(o);
					});

				if (_traceRedis)
					tracing.AddRedisInstrumentation(o =>
					{
						o.SetVerboseDatabaseStatements = true;
						o.Enrich = (activity, command) => { activity.DisplayName = $"Redis ({command.Command})"; };
						RedisInstrumentation?.Invoke(o);
					});

				if (_traceMongo) tracing.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");
			})
			.WithMetrics(metric =>
			{
				metric.AddOtlpExporter(o =>
					{
						o.Endpoint = OtCollectorUri;
					})
					.AddPrometheusExporter()
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