using RestfulProcessControl.Models;

namespace RestfulProcessControl;

public class Application
{
	public string FolderName { get; }
	public ApplicationConfigModel? Config { get; private set; }
	public bool Running { get; private set; }

	public Application()
	{
		FolderName = "Testapp";
		Config = null;
		Running = false;
	}

	/// <summary>
	/// Reloads the application configuration from the application directory
	/// </summary>
	public void ReloadConfig() => Config = LoadConfig();

	/// <summary>
	/// Loads the configuration from the application directory
	/// </summary>
	/// <returns>A Configuration if one could be loaded</returns>
	private ApplicationConfigModel? LoadConfig() {
		Logger.Log(LogLevel.Information, "Loading Configuration for \"{0}\"...", FolderName);
		var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "apps", FolderName, "config.json");
		if (!System.IO.File.Exists(jsonFilePath)) return null;
		var jsonString = System.IO.File.ReadAllText(jsonFilePath);
		return ApplicationConfigModel.Deserialize(jsonString);
	}

	/// <summary>
	/// Starts the application
	/// </summary>
	/// <returns>True, if successful, false otherwise</returns>
	public bool Start()
	{
		Logger.Log(LogLevel.Information, "Starting application \"{0}\"...", FolderName);
		Running = false;
		return false;
	}

	/// <summary>
	/// Stops the application
	/// </summary>
	/// <returns>True if successful, false otherwise</returns>
	public bool Stop()
	{
		Logger.Log(LogLevel.Information, "Stopping application \"{0}\"...", FolderName);
		Running = false;
		return false;
	}
}