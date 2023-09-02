using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Common.Extensions;
using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Repositories;
using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Services;
using Elyspio.OpenTelemetry.Examples.WebApi.Assemblers;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Base;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Transports;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Services;

public class TodoService(ITodoRepository todoRepository, IUserService userService, ILogger<TodoService> logger) : TracingService(logger), ITodoService
{
	private readonly TodoAssembler todoAssembler = new();


	public async Task<Todo> Add(Guid idUser, string label)
	{
		using var _ = LogService($"{Log.F(idUser)} {Log.F(label)}");

		var entity = await todoRepository.Add(new TodoBase
		{
			Checked = false,
			Label = label,
			User = await userService.GetUsername(idUser)
		});

		return todoAssembler.Convert(entity);
	}

	public async Task<List<Todo>> GetAll(Guid idUser)
	{
		using var _ = LogService($"{Log.F(idUser)}");

		var entities = await todoRepository.GetAll();

		return todoAssembler.Convert(entities);
	}


	public async Task Delete(Guid idUser, Guid idTodo)
	{
		using var _ = LogService($"{Log.F(idUser)} {Log.F(idTodo)}");

		var todo = await todoRepository.GetById(idTodo.AsObjectId());

		var username = await userService.GetUsername(idUser);

		if (todo is null) throw new Exception("User not found");

		if (username != todo.User) throw new Exception("This is not your todo");

		await todoRepository.Delete(idTodo.AsObjectId());
	}
}