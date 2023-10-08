namespace Elyspio.OpenTelemetry.Constants;

public class MetterConstants
{
	public static readonly string[] DefaultMetters =
	{
		"Microsoft.AspNetCore.Hosting",
		"Microsoft.AspNetCore.Http",
		"Microsoft.AspNetCore.Http.Connections",
		"Microsoft.AspNetCore.Server.Kestrel",
		"System.Net.NameResolution",
		"System.Net.Security",
		"System.Net.Sockets"
	};
}