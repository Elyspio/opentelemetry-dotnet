using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Services;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Transports;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements;
using Microsoft.AspNetCore.Mvc;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Controllers;

[Route("api/todo")]
[ApiController]
public class TodoController(ITodoService todoService, ILogger<TodoController> logger) : TracingController(logger)
{
	[HttpGet]
	[ProducesResponseType(typeof(List<Todo>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(Guid idUser)
	{
		using var _ = LogController();
		return Ok(await todoService.GetAll(idUser));
	}


	[HttpPost]
	[ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
	public async Task<IActionResult> Add(Guid idUser, [FromBody] string label)
	{
		using var _ = LogController($"{Log.F(label)}");
		var todo = await todoService.Add(idUser, label);
		return Created($"api/todo/{todo.Id}", todo);
	}

	[HttpDelete("{id:guid}")]
	[ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Delete(Guid idUser, Guid id)
	{
		using var _ = LogController($"{Log.F(id)}");
		await todoService.Delete(idUser, id);
		return NoContent();
	}
}