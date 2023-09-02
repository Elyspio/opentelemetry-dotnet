using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Repositories;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Base;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Entities;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements;
using Microsoft.EntityFrameworkCore;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Repositories.Sql;

public class UserRepository(ILogger<UserRepository> logger, AppSqlContext context) : TracingRepository(logger), IUserRepository
{
	public async Task<UserEntity> Add(UserBase @base)
	{
		using var logger = LogRepository($"{Log.F(@base)}", autoExit: false);

		var result = await context.Users.AddAsync(new UserEntity()
		{
			Id = Guid.NewGuid(),
			Username = @base.Username
		});

		await context.SaveChangesAsync();

		logger.Exit($"{Log.F(result.Entity.Id)}");

		return result.Entity;
	}


	public async Task<List<UserEntity>> GetAll()
	{
		using var logger = LogRepository();

		return await context.Users.ToListAsync();
	}

	public async Task Delete(Guid id)
	{
		using var logger = LogRepository($"{Log.F(id)}", autoExit: false);

		var deletedCount = await context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();

		await context.SaveChangesAsync();


		logger.Exit($"{Log.F(deletedCount)}");
	}

	public async Task<UserEntity?> GetById(Guid id)
	{
		using var logger = LogRepository($"{Log.F(id)}");

		return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
	}
}