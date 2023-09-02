using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Models;
using Elyspio.OpenTelemetry.Examples.WebApi.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Models.Entities;

public class TodoEntity : TodoBase, IEntity
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public ObjectId Id { get; set; }
}