using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Repositories.Mongo.Technical;

/// <summary>
///     Manage app mongo connection
/// </summary>
public sealed class MongoContext
{
	static MongoContext()
	{
		var pack = new ConventionPack
		{
			new EnumRepresentationConvention(BsonType.String)
		};
		ConventionRegistry.Register("EnumStringConvention", pack, _ => true);
		BsonSerializer.RegisterSerializationProvider(new EnumAsStringSerializationProvider());
	}

	/// <summary>
	///     Default constructor
	/// </summary>
	/// <param name="configuration"></param>
	public MongoContext(IConfiguration configuration)
	{
		var connectionString = configuration["Mongo"];

		ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

		var (client, url) = MongoClientFactory.Create(connectionString);

		Console.WriteLine($"Connecting to Database '{url.DatabaseName}'");

		MongoDatabase = client.GetDatabase(url.DatabaseName);
	}

	/// <summary>
	///     Récupération de la IMongoDatabase
	/// </summary>
	/// <returns></returns>
	public IMongoDatabase MongoDatabase { get; }
}