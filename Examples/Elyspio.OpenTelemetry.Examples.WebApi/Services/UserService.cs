using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Repositories;
using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Services;
using Elyspio.OpenTelemetry.Examples.WebApi.Assemblers;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Base;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Transports;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Elyspio.OpenTelemetry.Tracing.Elements;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Services;

public class UserService : TracingService, IUserService
{
	private readonly IUserRepository _userRepository;
	private readonly UserAssembler _userAssembler = new();

	public UserService(ILogger<UserService> logger, IUserRepository userRepository) : base(logger)
	{
		_userRepository = userRepository;
	}

	public async Task Delete(Guid idUser)
	{
		using var _ = LogService($"{Log.F(idUser)}");

		await _userRepository.Delete(idUser);
	}

	public async Task<User> Add(string username)
	{
		using var _ = LogService($"{Log.F(username)}");

		var entity = await _userRepository.Add(new UserBase
		{
			Username = username
		});

		return _userAssembler.Convert(entity);
	}

	public async Task<List<User>> GetAll()
	{
		using var _ = LogService();

		var entities = await _userRepository.GetAll();

		return _userAssembler.Convert(entities);
	}

	public async Task<User> GetById(Guid idUser)
	{
		using var _ = LogService($"{Log.F(idUser)}");

		var entity = await _userRepository.GetById(idUser);

		if (entity is null) throw new Exception("User not found");

		return _userAssembler.Convert(entity);
	}

	public async Task<string> GetUsername(Guid idUser)
	{
		using var _ = LogService($"{Log.F(idUser)}");

		var user = await _userRepository.GetById(idUser);

		if (user is null) throw new Exception("User not found");

		return user.Username;
	}
}