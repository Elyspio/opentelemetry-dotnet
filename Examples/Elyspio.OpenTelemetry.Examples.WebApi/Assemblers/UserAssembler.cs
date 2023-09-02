using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Common.Assemblers;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Entities;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Transports;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Assemblers;

public class UserAssembler : BaseAssembler<User, UserEntity>
{
	public override UserEntity Convert(User obj)
	{
		return new UserEntity()
		{
			Username = obj.Username,
			Id = obj.Id
		};
	}

	public override User Convert(UserEntity obj)
	{
		return new User()
		{
			Username = obj.Username,
			Id = obj.Id
		};
	}
}