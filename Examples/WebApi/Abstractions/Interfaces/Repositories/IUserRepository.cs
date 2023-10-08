using Elyspio.OpenTelemetry.Examples.WebApi.Models.Base;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Entities;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Repositories;

public interface IUserRepository : ICrudSqlRepository<UserEntity, UserBase>
{
}