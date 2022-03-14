using System.IO.Compression;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Models;

namespace RestfulProcessControl.Controllers;

[ApiController]
[Route("Apps")]
public class ApplicationsController : ControllerBase
{
	private static readonly List<Application> Apps;
	static ApplicationsController() => Apps = new List<Application> { new() };

	/// <summary>
	/// Get all Applications
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <returns>200OK and an array of Applications if it was successful, 403Forbidden otherwise</returns>
	[RequireHttps]
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Application>))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetAll([FromQuery]string jwt)
	{
		if (!Authenticator.IsTokenValid(jwt, out _)) return Forbid();
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
		if (!Authenticator.IsTokenValid(jwt, out _)) return Forbid();

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
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Application))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetById([FromQuery]string jwt, [FromRoute]int id)
	{
		if (!Authenticator.IsTokenValid(jwt, out _)) return Forbid();

		if (id >= Apps.Count) return NotFound();
		return Ok(Apps[id]);
	}

	[RequireHttps]
	[HttpPost("{id:int}/backup")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> CreateBackup([FromQuery] string jwt, [FromRoute] int id)
	{
		if (!Authenticator.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.CreateBackup))
			return Forbid();
		var found = await CreateBackup(id);
		return found ? Ok() : NotFound();
	}

	private static Task<bool> CreateBackup(int id)
	{
		Logger.Log(LogLevel.Information, "Creating and Downloading Backup of application {0}...", id);

		if (id >= Apps.Count) return Task.FromResult(false);
		var backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "apps", Apps[id].FolderName, "application");

		if (!Directory.Exists(backupFolderPath)) return Task.FromResult(false);
		var zipPath = Path.Combine(backupFolderPath, "..", "backup-" + Apps[id].FolderName + ".zip");

		if (System.IO.File.Exists(zipPath))
		{
			var oldZipPath = zipPath + "-old";
			if (System.IO.File.Exists(oldZipPath)) System.IO.File.Delete(oldZipPath);
			System.IO.File.Move(zipPath, oldZipPath);
		}

		ZipFile.CreateFromDirectory(backupFolderPath, zipPath);
		Logger.Log(LogLevel.Information, "Backup created!");
		return Task.FromResult(true);
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
	public IActionResult GetBackup([FromQuery] string jwt, [FromRoute] int id)
	{
		if (!Authenticator.IsTokenValid(jwt, out _)) return Forbid();
		var zipPath =
			Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "apps", Apps[id].FolderName, "application"),
				"..", "backup-" + Apps[id].FolderName + ".zip");
		return File(System.IO.File.OpenRead(zipPath), "application/zip", $"{Apps[id].FolderName}-backup.zip");
	}
}