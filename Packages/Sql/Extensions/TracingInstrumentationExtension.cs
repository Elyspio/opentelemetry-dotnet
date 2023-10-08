using Elyspio.OpenTelemetry.Sql.Helpers;
using Microsoft.Data.SqlClient;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Trace;

namespace Elyspio.OpenTelemetry.Sql.Extensions;

/// <summary>
///     Extensions for sql client instrumentation
/// </summary>
public static class TracingInstrumentationExtension
{
	/// <summary>
	///     Add sql client instrumentation
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="action"></param>
	/// <returns></returns>
	public static TracerProviderBuilder AddAppSqlClientInstrumentation(this TracerProviderBuilder builder, Action<SqlClientInstrumentationOptions>? action = null)
	{
		builder.AddSqlClientInstrumentation(o =>
		{
			o.EnableConnectionLevelAttributes = true;
			o.RecordException = true;
			o.SetDbStatementForText = true;
			o.SetDbStatementForStoredProcedure = true;
			o.Enrich = (activity, s, arg3) =>
			{
				if (arg3 is not SqlCommand command) return;

				var tables = SqlHelper.ExtractTablesFromQuery(command.CommandText.AsSpan());
				tables.Sort();
				activity.DisplayName = $"SQL - {command.Connection.Database} - {string.Join(", ", tables)}";
			};
			action?.Invoke(o);
		});
		return builder;
	}
}