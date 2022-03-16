﻿using RestfulProcessControl.Util.JsonContext;
using System.Text.Json;

namespace RestfulProcessControl.Models;

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

public class ApplicationConfigModel
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

	public ApplicationConfigModel()
	{
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
	public static string Serialize(ApplicationConfigModel data) => JsonSerializer.Serialize(data, ApplicationConfigModelJsonContext.Default.ApplicationConfigModel);

	public static ApplicationConfigModel? Deserialize(string data)
	{
		try { return JsonSerializer.Deserialize(data, ApplicationConfigModelJsonContext.Default.ApplicationConfigModel); }
		catch { return null; }
	}
}