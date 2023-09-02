using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Repositories.Mongo.Technical;

/// <summary>
///     Manage mongo client
/// </summary>
public static class MongoClientFactory
{
	/// <summary>
	///     Create mongo client with telemetry support
	/// </summary>
	/// <param name="connectionString"></param>
	/// <returns></returns>
	public static (MongoClient Client, MongoUrl Url) Create(string connectionString)
	{
		var mongoUrl = new MongoUrl(connectionString);
		var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
		clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber(new InstrumentationOptions
		{
			CaptureCommandText = true
		}));

		return (new MongoClient(clientSettings), mongoUrl);
	}
}