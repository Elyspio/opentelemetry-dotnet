using Elyspio.OpenTelemetry.Examples.WebApi.Models.Transports;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Services;

public interface ITodoService
{
	Task<Todo> Add(Guid idUser, string label);
	Task<List<Todo>> GetAll(Guid idUser);
	Task Delete(Guid idUser, Guid idTodo);
}