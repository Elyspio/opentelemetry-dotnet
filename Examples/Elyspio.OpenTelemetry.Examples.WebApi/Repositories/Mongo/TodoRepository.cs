using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Repositories;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Base;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Entities;
using Elyspio.OpenTelemetry.Examples.WebApi.Repositories.Mongo.Base;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Repositories.Mongo;

internal class TodoRepository(IConfiguration configuration, ILogger<BaseRepository<TodoEntity>> logger) : CrudMongoRepository<TodoEntity, TodoBase>(configuration, logger), ITodoRepository
{
}