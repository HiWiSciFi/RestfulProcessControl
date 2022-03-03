using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

namespace RestfulProcessControl.Controllers;

[ApiController]
[Route("Apps")]
public class ApplicationsHttpController : ControllerBase
{
	private static readonly List<ApplicationManager> Apps;
	static ApplicationsHttpController() => Apps = new List<ApplicationManager> { new() };

	/// <summary>
	/// Get all Applications
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <returns>200OK and an array of Applications if it was successful, 403Forbidden otherwise</returns>
	[RequireHttps]
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ApplicationManager>))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetAll([FromQuery]string jwt)
	{
		if (!Authenticator.IsTokenValid(jwt)) return Forbid();
		return Ok(Apps);
	}

	/// <summary>
	/// Reload the configuration file of a specific application
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="id">The ID of the application to reload the configuration for</param>
	/// <returns>200OK if the configuration was reloaded, 303NotFound if the specified
	/// application does not exist, 403Forbidden otherwise</returns>
	[RequireHttps]
	[HttpPatch("{id:int}/reload")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult ReloadApplicationConfig([FromQuery]string jwt, [FromRoute]int id)
	{
		if (!Authenticator.IsTokenValid(jwt)) return Forbid();

		if (id >= Apps.Count) return NotFound();
		Apps[id].ReloadConfig();
		return Ok();
	}

	/// <summary>
	/// Gets a specific Application
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="id">The ID of the Application</param>
	/// <returns>200OK and an Application if successful, 404NotFound if the application
	/// does not exist, 403Forbidden otherwise</returns>
	[RequireHttps]
	[HttpGet("{id:int}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationManager))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetById([FromQuery]string jwt, [FromRoute]int id)
	{
		if (!Authenticator.IsTokenValid(jwt)) return Forbid();

		if (id >= Apps.Count) return NotFound();
		return Ok(Apps[id]);
	}

	/// <summary>
	/// Downloads the latest backup of an application
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="id">The ID of the application</param>
	/// <returns>A File stream for the backup file</returns>
	[RequireHttps]
	[HttpGet("{id:int}/download")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStream))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetBackup([FromQuery]string jwt, [FromRoute]int id)
	{
		if (!Authenticator.IsTokenValid(jwt)) return Forbid();

		Logger.Log(LogLevel.Information, "Creating and Downloading Backup of application {id}...", id);

		if (id >= Apps.Count) return NotFound();
		var backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "apps", Apps[id].FolderName, "application");

		if (!Directory.Exists(backupFolderPath)) return NotFound();
		var zipPath = Path.Combine(backupFolderPath, "..", "backup-" + Apps[id].FolderName + ".zip");

		if (System.IO.File.Exists(zipPath)) {
			var oldZipPath = zipPath + "-old";
			if (System.IO.File.Exists(oldZipPath)) System.IO.File.Delete(oldZipPath);
			System.IO.File.Move(zipPath, oldZipPath);
		}

		ZipFile.CreateFromDirectory(backupFolderPath, zipPath);
		return File(System.IO.File.OpenRead(zipPath), "application/zip", "backup.zip");
	}
}