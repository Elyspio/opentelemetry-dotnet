using System.ComponentModel.DataAnnotations;
using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Models;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Base;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Models.Transports;

public class Todo : TodoBase, ITransport
{
	[Required] public required Guid Id { get; init; }
}