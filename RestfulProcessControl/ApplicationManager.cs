using System.IO.Compression;
using RestfulProcessControl.Util;

namespace RestfulProcessControl;

public static class ApplicationManager
{
	private static List<Application> Applications { get; }
	static ApplicationManager() => Applications = new List<Application>();

	/// <summary>
	/// Loads all Applications
	/// </summary>
	public static void LoadAll()
	{

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
		var backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "apps", Applications[id].FolderName, "application");

		if (!Directory.Exists(backupFolderPath)) return Task.FromResult(false);
		var zipPath = Path.Combine(backupFolderPath, "..", "backup-" + Applications[id].FolderName + ".zip");

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

	public static Task<FileStream?> GetBackupStream(int id)
	{
		var zipPath =
			Path.Combine(
				Path.Combine(Directory.GetCurrentDirectory(), "apps", Applications[id].FolderName, "application"), "..",
				"backup-" + Applications[id].FolderName + ".zip");
		return !File.Exists(zipPath)
			? Task.FromResult<FileStream?>(null)
			: Task.FromResult<FileStream?>(File.OpenRead(zipPath));
	}
}