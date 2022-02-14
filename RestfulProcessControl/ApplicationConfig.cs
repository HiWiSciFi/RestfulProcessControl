using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestfulProcessControl;

// information required to start a process and interact with it:
// 
// fileName : string
// arguments : string
// shutdownType : string --> [ "kill", "stdin" ]
// shutdownStdinString : string
// preStartupShellCalls : string[]
// postStartupShellCalls : string[]
// postStartupCalls : string[]
// preShutdownShellCalls : string[]
// preShutdownCalls : string[]
// postShutdownShellCalls : string[]
// restartOnCrash : bool

public struct ApplicationConfig
{
	public string? FileName { get; set; }
	public string? Arguments { get; set; }
	public bool RestartOnCrash { get; set; }

	public string[]? PreStartupShellCalls { get; set; }
	public string[]? PostStartupShellCalls { get; set; }
	public string[]? PostStartupStdinCalls { get; set; }

	public string? ShutdownType { get; set; }
	public string? ShutdownStdinString { get; set; }
	public string[]? PreShutdownShellCalls { get; set; }
	public string[]? PreShutdownStdinCalls { get; set; }
	public string[]? PostShutdownShellCalls { get; set; }
	public string[]? PostShutdownStdinCalls { get; set; }

	public ApplicationConfig() {
		FileName = null;
		Arguments = null;
		RestartOnCrash = false;
		PreStartupShellCalls = null;
		PostStartupShellCalls = null;
		PostStartupStdinCalls = null;
		ShutdownType = null;
		ShutdownStdinString = null;
		PreShutdownShellCalls = null;
		PreShutdownStdinCalls = null;
		PostShutdownShellCalls = null;
		PostShutdownStdinCalls = null;
	}

	public string Serialize() => Serialize(this);
	public static string Serialize(ApplicationConfig data) => JsonSerializer.Serialize(data, ApplicationConfigJsonContext.Default.ApplicationConfig);

	public static ApplicationConfig? Deserialize(string data) {
		try { return JsonSerializer.Deserialize(data, ApplicationConfigJsonContext.Default.ApplicationConfig); }
		catch { return null; }
	}
}

[JsonSerializable(typeof(ApplicationConfig))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class ApplicationConfigJsonContext : JsonSerializerContext { }