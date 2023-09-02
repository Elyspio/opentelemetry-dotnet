# Elyspio.Telemetry

This package allows adding telemetry information to a .NET 7+ application.

```bash
dotnet add package Elyspio.Telemetry
```

## Usage

### Activation in the application

In the `Startup.cs` file or its equivalent in .NET 6++:

```csharp   
var telemetryBuilder = new AppOpenTelemetryBuilder<Program>("my-service-name", traceSql: true, traceMongo: true, traceRedis: true)
{
    // OpenTelemetry agent collector URL
    OtCollectorUri = new Uri("http://localhost:4317")
};

telemetryBuilder.Build(builder.Services);
```

### MongoDB Trace Management

To access queries executed in MongoDB, add this code during client creation:

```csharp
var mongoUrl = new MongoUrl(connectionString);
var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber(new InstrumentationOptions
{
    CaptureCommandText = true
}));

var client = new MongoClient(clientSettings);
```

## Example

In the [Examples](./Examples) directory, you can find a WebApi project using this package.
