# Elyspio.Telemetry

This package allows adding telemetry information to a .NET 7+ application.

```bash
dotnet add package Elyspio.Telemetry
```

## Usage

### Activation in the application

In the `Startup.cs` file or its equivalent in .NET 6++:

```csharp   
var telemetryBuilder = new AppOpenTelemetryBuilder<Program>(new AppOpenTelemetryBuilderOptions("elyspio-telemetry-tests-webapi"))
{
	OtCollectorUri = new Uri("http://localhost:4317"),
	Tracing = tracing => tracing
		.AddAppMongoInstrumentation() // Elyspio.Telemetry.MongoDB
		.AddAppSqlClientInstrumentation() // Elyspio.Telemetry.Sql
		.AddAppRedisInstrumentation() // Elyspio.Telemetry.Redis
};

telemetryBuilder.Build(builder.Services);
```

### MongoDB Trace Management

To access queries executed in MongoDB, add this code during client creation:

```csharp
var mongoUrl = new MongoUrl(connectionString);
var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
clientSettings.ClusterConfigurator = cb =>
{
    cb.Subscribe(new MongoDbActivityEventSubscriber());
}

var client = new MongoClient(clientSettings);
```

## Example

In the [Examples](./Examples) directory, you can find a WebApi project using this package.

## Publish package

In the [Packages](./Packages) directory, you can find a script to publish the package to NuGet.

Usage : `./publish.sh -pat <PAT>` or `./publish.sh --version 2.0.0 --pat <PAT>` to specify a version.