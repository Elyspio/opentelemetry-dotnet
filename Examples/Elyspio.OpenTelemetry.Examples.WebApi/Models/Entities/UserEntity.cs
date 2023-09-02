using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Models;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Base;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Models.Entities;

public class UserEntity : UserBase, ISqlEntity
{
	public Guid Id { get; set; }
}