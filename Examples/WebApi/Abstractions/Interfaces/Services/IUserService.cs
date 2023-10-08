using Elyspio.OpenTelemetry.Examples.WebApi.Models.Transports;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Services;

public interface IUserService
{
	Task Delete(Guid idUser);
	Task<User> Add(string username);
	Task<List<User>> GetAll();
	Task<User> GetById(Guid idUser);

	Task<string> GetUsername(Guid idUser);
}