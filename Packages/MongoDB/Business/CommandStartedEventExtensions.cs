using MongoDB.Driver.Core.Events;

namespace Elyspio.OpenTelemetry.MongoDB.Business;

/// <summary>
///     Helper for <see cref="CommandStartedEvent" />
/// </summary>
public static class CommandStartedEventExtensions
{
	private static readonly HashSet<string> CommandsWithCollectionNameAsValue =
		new()
		{
			"aggregate",
			"count",
			"distinct",
			"mapReduce",
			"geoSearch",
			"delete",
			"find",
			"killCursors",
			"findAndModify",
			"insert",
			"update",
			"create",
			"drop",
			"createIndexes",
			"listIndexes"
		};

    /// <summary>
    ///     Get the collection name from a command
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public static string? GetCollectionName(this CommandStartedEvent @event)
	{
		if (@event.CommandName == "getMore")
		{
			if (!@event.Command.Contains("collection")) return null;

			var collectionValue = @event.Command.GetValue("collection");
			if (collectionValue.IsString) return collectionValue.AsString;
		}
		else if (CommandsWithCollectionNameAsValue.Contains(@event.CommandName))
		{
			var commandValue = @event.Command.GetValue(@event.CommandName);
			if (commandValue != null && commandValue.IsString) return commandValue.AsString;
		}

		return null;
	}
}