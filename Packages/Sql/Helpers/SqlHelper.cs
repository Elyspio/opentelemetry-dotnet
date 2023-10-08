namespace Elyspio.OpenTelemetry.Sql.Helpers;

internal static class SqlHelper
{
	internal static List<string> ExtractTablesFromQuery(ReadOnlySpan<char> query)
	{
		var indexOfFrom = query.IndexOf(Keywords.From);
		var afterFromIndex = indexOfFrom + Keywords.From.Length;

		var indexOfWhere = query.IndexOf(Keywords.Where);
		var indexOfGroupBy = query.IndexOf(Keywords.GroupBy);

		var indexToStop = query.Length;
		if (indexOfWhere != -1) indexToStop = indexOfWhere;
		if (indexOfGroupBy != -1 && indexOfGroupBy < indexToStop) indexToStop = indexOfGroupBy;


		var fromPattern = query.Slice(afterFromIndex, indexToStop - afterFromIndex);

		var tables = new List<string>();

		var lastFoundIndex = 0;
		for (var i = 0; i < fromPattern.Length; i++)
		{
			if (i + 1 != fromPattern.Length && fromPattern[i] != ',') continue;

			var part = fromPattern.Slice(lastFoundIndex, i + 1);
			var indexOfAs = part.IndexOf(Keywords.As);
			tables.Add(part[..indexOfAs].ToString().Trim(' ', '[', ']'));
			lastFoundIndex = i;
		}

		return tables;
	}

	private static class Keywords
	{
		public const string From = "FROM";
		public const string Where = "WHERE";
		public const string GroupBy = "GROUP BY";
		public const string As = "AS";
	}
}