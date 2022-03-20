using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Managers;
using RestfulProcessControl.Models;

namespace RestfulProcessControl.Controllers;

[ApiController]
[Route("Apps")]
public class ApplicationsController : ControllerBase
{
	/// <summary>
	/// Get all Applications
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <returns>200OK and an array of Applications if it was successful, 403Forbidden otherwise</returns>
	[RequireHttps]
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Application>))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetAll([FromQuery] string jwt)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.GetApplication))
			return Forbid();
		return Ok(ApplicationManager.GetApplications());
	}

	/// <summary>
	/// Reload the configuration file of a specific application
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="id">The ID of the application to reload the configuration for</param>
	/// <returns>200OK if the configuration was reloaded, 303NotFound if the specified
	/// application does not exist, 403Forbidden otherwise</returns>
	[RequireHttps]
	[HttpPatch("App/{id:int}/reload")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> ReloadApplicationConfig([FromQuery] string jwt, [FromRoute] int id)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.EditApplication))
			return Forbid();
		if (ApplicationManager.GetApp(id) is null) return NotFound();
		return await ApplicationManager.ReloadConfig(id) ? Ok() : UnprocessableEntity();
	}

	/// <summary>
	/// Gets a specific Application
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="id">The ID of the Application</param>
	/// <returns>200OK and an Application if successful, 404NotFound if the application
	/// does not exist, 403Forbidden otherwise</returns>
	[RequireHttps]
	[HttpGet("App/{id:int}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Application))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetById([FromQuery] string jwt, [FromRoute] int id)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.GetApplication))
			return Forbid();
		var app = ApplicationManager.GetApp(id);
		return app is null ? NotFound() : Ok(app);
	}

	/// <summary>
	/// Creates a backup for an application
	/// </summary>
	/// <param name="jwt"></param>
	/// <param name="id"></param>
	/// <returns></returns>
	[RequireHttps]
	[HttpPost("App/{id:int}/backup")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> CreateBackup([FromQuery] string jwt, [FromRoute] int id)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.CreateBackup))
			return Forbid();
		var found = await ApplicationManager.CreateBackup(id);
		return found ? Ok() : NotFound();
	}

	/// <summary>
	/// Downloads the latest backup of an application
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="id">The ID of the application</param>
	/// <returns>A File stream for the backup file</returns>
	[RequireHttps]
	[HttpGet("App/{id:int}/backup")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStream))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetBackup([FromQuery] string jwt, [FromRoute] int id)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.DownloadBackup))
			return Forbid();
		var fs = await ApplicationManager.GetBackupStream(id);
		if (fs is null) return NotFound();
		return File(fs, "application/zip", $"{ApplicationManager.GetApp(id)!.Name}-backup.zip");
	}
}