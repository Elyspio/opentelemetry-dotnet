using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Repositories;
using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Services;
using Elyspio.OpenTelemetry.Examples.WebApi.Repositories.Mongo;
using Elyspio.OpenTelemetry.Examples.WebApi.Repositories.Sql;
using Elyspio.OpenTelemetry.Examples.WebApi.Services;
using Elyspio.OpenTelemetry.MongoDB.Extensions;
using Elyspio.OpenTelemetry.Redis.Extensions;
using Elyspio.OpenTelemetry.Sql.Extensions;
using Elyspio.OpenTelemetry.Technical.Options;
using Elyspio.OpenTelemetry.Tracing.Builder;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSqlServer<AppSqlContext>(builder.Configuration["Sql"]);


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var telemetryBuilder = new AppOpenTelemetryBuilder<Program>(new AppOpenTelemetryBuilderOptions("http://localhost:4317", "elyspio-telemetry-tests-webapi"))
{
	Tracing = tracing => tracing
		.AddAppMongoInstrumentation()
		.AddAppSqlClientInstrumentation()
		.AddAppRedisInstrumentation()
};
telemetryBuilder.Build(builder.Services);


builder.Services.AddControllers();

var app = builder.Build();

var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<AppSqlContext>();
dbContext.Database.EnsureCreated();


app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();