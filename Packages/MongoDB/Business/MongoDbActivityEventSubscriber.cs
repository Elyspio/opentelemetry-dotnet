using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Elyspio.OpenTelemetry.MongoDB.Options;
using MongoDB.Driver.Core.Events;

namespace Elyspio.OpenTelemetry.MongoDB.Business;

/// <summary>
///     Capture mongodb events and create activities
/// </summary>
public class MongoDbActivityEventSubscriber : IEventSubscriber
{
	private const string ActivityName = "MongoDB.Driver.Core.Events.Command";
	private static readonly AssemblyName AssemblyName = typeof(MongoDbActivityEventSubscriber).Assembly.GetName();

	/// <summary>
	///     The name of the activity source.
	/// </summary>
	public static readonly string ActivitySourceName = AssemblyName.Name!;

	private static readonly Version Version = AssemblyName.Version!;
	private static readonly ActivitySource ActivitySource = new(ActivitySourceName, Version.ToString());
	private readonly ConcurrentDictionary<int, Activity> _activityMap = new();
	private readonly MongoInstrumentationOptions _options;

	/// <inheritdoc />
	public MongoDbActivityEventSubscriber() : this(new MongoInstrumentationOptions { CaptureCommandText = true })
	{
	}

	/// <summary>
	///     Create a new instance of <see cref="MongoDbActivityEventSubscriber" />
	/// </summary>
	/// <param name="options"></param>
	public MongoDbActivityEventSubscriber(MongoInstrumentationOptions options)
	{
		_options = options;
	}

	/// <summary>
	///     Subscribe to mongodb events
	/// </summary>
	/// <param name="handler"></param>
	/// <typeparam name="TEvent"></typeparam>
	/// <returns></returns>
	public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
	{
		if (typeof(TEvent) == typeof(CommandStartedEvent))
		{
			handler = (Action<TEvent>)(object)new Action<CommandStartedEvent>(HandleCommandStarted);
			return true;
		}

		if (typeof(TEvent) == typeof(CommandSucceededEvent))
		{
			handler = (Action<TEvent>)(object)new Action<CommandSucceededEvent>(HandleCommandSucceeded);
			return true;
		}

		if (typeof(TEvent) == typeof(CommandFailedEvent))
		{
			handler = (Action<TEvent>)(object)new Action<CommandFailedEvent>(HandleCommandFailed);
			return true;
		}

		handler = null!;
		return false;
	}

	private void HandleCommandStarted(CommandStartedEvent @event)
	{
		if (_options.ShouldStartActivity?.Invoke(@event) == false) return;

		// ReSharper disable once ExplicitCallerInfoArgument
		var activity = ActivitySource.StartActivity(ActivityName, ActivityKind.Client);

		if (activity == null) return;

		var collectionName = @event.GetCollectionName();

		activity.DisplayName = collectionName == null ? $"MongoDB - {@event.CommandName}" : $"MongoDB - {collectionName} - {@event.CommandName}";

		AddTags(@event, activity, collectionName);

		_activityMap.TryAdd(@event.RequestId, activity);
	}


	private void HandleCommandSucceeded(CommandSucceededEvent @event)
	{
		if (_activityMap.TryRemove(@event.RequestId, out var activity))
			WithReplacedActivityCurrent(activity, () =>
			{
				activity.AddTag("otel.status_code", "OK");
				activity.SetStatus(ActivityStatusCode.Ok);
				activity.Stop();
			});
	}

	private void HandleCommandFailed(CommandFailedEvent @event)
	{
		if (_activityMap.TryRemove(@event.RequestId, out var activity))
			WithReplacedActivityCurrent(activity, () =>
			{
				if (activity.IsAllDataRequested) AddTags(@event, activity);

				activity.SetStatus(ActivityStatusCode.Error);
				activity.Stop();
			});
	}

	private static void WithReplacedActivityCurrent(Activity activity, Action action)
	{
		var current = Activity.Current;
		try
		{
			Activity.Current = activity;
			action();
		}
		finally
		{
			Activity.Current = current;
		}
	}

	#region Add Tags

	private static void AddTags(CommandFailedEvent @event, Activity activity)
	{
		activity.AddTag("otel.status_code", "ERROR");
		activity.AddTag("otel.status_description", @event.Failure.Message);
		activity.AddTag("exception.type", @event.Failure.GetType().FullName);
		activity.AddTag("exception.message", @event.Failure.Message);
		activity.AddTag("exception.stacktrace", @event.Failure.StackTrace);
	}

	private void AddTags(CommandStartedEvent @event, Activity activity, string? collectionName)
	{
		activity.AddTag("db.system", "mongodb");
		activity.AddTag("db.connection_id", @event.ConnectionId?.ToString());
		activity.AddTag("db.name", @event.DatabaseNamespace?.DatabaseName);
		activity.AddTag("db.mongodb.collection", collectionName);
		activity.AddTag("db.operation", @event.CommandName);

		var endPoint = @event.ConnectionId?.ServerId?.EndPoint;
		switch (endPoint)
		{
			case IPEndPoint ipEndPoint:
				activity.AddTag("net.peer.port", ipEndPoint.Port.ToString());
				activity.AddTag("net.sock.peer.addr", ipEndPoint.Address.ToString());
				break;
			case DnsEndPoint dnsEndPoint:
				activity.AddTag("net.peer.name", dnsEndPoint.Host);
				activity.AddTag("net.peer.port", dnsEndPoint.Port.ToString());
				break;
		}

		if (activity.IsAllDataRequested && _options.CaptureCommandText) activity.AddTag("db.statement", @event.Command.ToString());
	}

	#endregion Add Tags
}