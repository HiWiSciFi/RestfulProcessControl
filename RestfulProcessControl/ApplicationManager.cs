using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Models;

namespace RestfulProcessControl;

public class ApplicationManager
{
	public string FolderName { get; }
	public ApplicationConfigModel? Config { get; private set; }
	public bool Running { get; private set; }

	public ILogger<ApplicationManager> _logger;
	public ApplicationManager()
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
		_logger.LogInformation("Loading Configuration for \"{0}\"...", FolderName);
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
		_logger.LogInformation("Starting application \"{0}\"...", FolderName);
		Running = false;
		return false;
	}

	/// <summary>
	/// Stops the application
	/// </summary>
	/// <returns>True if successful, false otherwise</returns>
	public bool Stop()
	{
		_logger.LogInformation("Stopping application \"{0}\"...", FolderName);
		Running = false;
		return false;
	}
}