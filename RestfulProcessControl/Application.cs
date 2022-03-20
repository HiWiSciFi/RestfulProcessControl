using RestfulProcessControl.Models;
using RestfulProcessControl.Util;

namespace RestfulProcessControl;

public class Application
{
	public string Name { get; }
	public ApplicationConfigModel? Config { get; private set; }
	public bool Running { get; private set; }

	public Application()
	{
		Name = "Testapp";
		Config = null;
		Running = false;
	}

	public Application(string name)
	{
		Name = name;
		Config = null;
		Running = false;
	}

	/// <summary>
	/// Reloads the application configuration from the application directory
	/// </summary>
	public async Task<ApplicationConfigModel?> ReloadConfig()
	{
		Config = await LoadConfig();
		return Config;
	}

	/// <summary>
	/// Loads the configuration from the application directory
	/// </summary>
	/// <returns>A Configuration if one could be loaded</returns>
	private Task<ApplicationConfigModel?> LoadConfig()
	{
		Logger.LogInformation("Loading Configuration for \"{0}\"...", Name);
		var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "apps", Name, "config.json");
		if (!System.IO.File.Exists(jsonFilePath)) return Task.FromResult<ApplicationConfigModel?>(null);
		var jsonString = System.IO.File.ReadAllText(jsonFilePath);
		return Task.FromResult(ApplicationConfigModel.Deserialize(jsonString));
	}

	/// <summary>
	/// Starts the application
	/// </summary>
	/// <returns>True, if successful, false otherwise</returns>
	public bool Start()
	{
		Logger.LogInformation("Starting application \"{0}\"...", Name);
		Running = false;
		return false;
	}

	/// <summary>
	/// Stops the application
	/// </summary>
	/// <returns>True if successful, false otherwise</returns>
	public bool Stop()
	{
		Logger.LogInformation("Stopping application \"{0}\"...", Name);
		Running = false;
		return false;
	}
}