using RestfulProcessControl.Util;
using System.IO.Compression;

namespace RestfulProcessControl.Managers;

public static class ApplicationManager
{
	private static List<Application> Applications { get; }
	static ApplicationManager() => Applications = new List<Application>();

	/// <summary>
	/// Loads all Applications
	/// </summary>
	/// <returns>true if the loading process was successful, false otherwise</returns>
	public static async Task<bool> LoadAll()
	{
		var appsFolderPath = Path.Combine(".", "apps");
		if (!Directory.Exists(appsFolderPath)) return false;
		var appFolders = Directory.GetDirectories(appsFolderPath);
		foreach (var appFolder in appFolders)
		{
			var appName = Path.GetDirectoryName(appFolder);
			if (appName is null or "") continue;
			if (!Directory.Exists(Path.Combine(appFolder, "application"))) continue;
			if (!File.Exists(Path.Combine(appFolder, "config.json"))) continue;

			var app = new Application(appName);
			var config = await app.ReloadConfig();
			if (config is null) continue;
			Applications.Add(app);
		}
		return true;
	}

	/// <summary>
	/// Get all applications
	/// </summary>
	/// <returns>A List of Applications</returns>
	public static IEnumerable<Application> GetApplications() => Applications;

	/// <summary>
	/// Get a specific application
	/// </summary>
	/// <param name="id">The id of the application</param>
	/// <returns>The application with the specified id or null if it does not exist</returns>
	public static Application? GetApp(int id) => id >= Applications.Count || id < 0 ? null : Applications[id];

	/// <summary>
	/// Asynchronously reloads the configuration file for an Application
	/// </summary>
	/// <param name="id">The id of the application</param>
	/// <returns>true if successful, false otherwise</returns>
	public static async Task<bool> ReloadConfig(int id)
	{
		var app = GetApp(id);
		if (app is null) return false;
		return await app.ReloadConfig() is null;
	}

	/// <summary>
	/// Asynchronously creates a backup file of an application
	/// </summary>
	/// <param name="id">The application ID</param>
	/// <returns>false if the application does not exist, true otherwise</returns>
	public static Task<bool> CreateBackup(int id)
	{
		Logger.LogInformation("Creating and Downloading Backup of application {0}...", id);

		if (id >= Applications.Count) return Task.FromResult(false);
		var backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "apps", Applications[id].Name, "application");

		if (!Directory.Exists(backupFolderPath)) return Task.FromResult(false);
		var zipPath = Path.Combine(backupFolderPath, "..", "backup-" + Applications[id].Name + ".zip");

		if (File.Exists(zipPath))
		{
			var oldZipPath = zipPath + "-old";
			if (File.Exists(oldZipPath)) File.Delete(oldZipPath);
			File.Move(zipPath, oldZipPath);
		}

		ZipFile.CreateFromDirectory(backupFolderPath, zipPath);
		Logger.LogInformation("Backup created!");
		return Task.FromResult(true);
	}

	/// <summary>
	/// Gets a File stream for the backup file of an application
	/// </summary>
	/// <param name="id">The id of the application</param>
	/// <returns></returns>
	public static Task<FileStream?> GetBackupStream(int id)
	{
		var zipPath =
			Path.Combine(
				Path.Combine(Directory.GetCurrentDirectory(), "apps", Applications[id].Name, "application"), "..",
				"backup-" + Applications[id].Name + ".zip");
		return !File.Exists(zipPath)
			? Task.FromResult<FileStream?>(null)
			: Task.FromResult<FileStream?>(File.OpenRead(zipPath));
	}
}