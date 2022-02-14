using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

namespace RestfulProcessControl.Controllers;

[ApiController]
[Route("Apps")]
public class ApplicationsController : ControllerBase
{
	private static readonly List<Application> apps;
	static ApplicationsController()
	{
		apps = new();
		apps.Add(new Application());
	}

	private readonly ILogger<ApplicationsController> _logger;
	public ApplicationsController(ILogger<ApplicationsController> logger) => _logger = logger;

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Application>))]
	public IActionResult GetAll() => Ok(apps);

	[HttpPatch("{id:int}/reload")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult ReloadApplicationConfig(int id) {
		if (id >= apps.Count) return NotFound();
		apps[id].ReloadConfig();
		return Ok();
	}

	[HttpGet("{id:int}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Application))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetById(int id)
	{
		if (id >= apps.Count) return NotFound();
		return Ok(apps[id]);
	}

	[HttpGet("{id:int}/download")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetBackup(int id) {
		if (id >= apps.Count) return NotFound();
		string backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "apps", apps[id].FolderName, "application");
		
		if (!Directory.Exists(backupFolderPath)) return NotFound();
		string zipPath = Path.Combine(backupFolderPath, "..", "backup-" + apps[id].FolderName + ".zip");
		
		if (System.IO.File.Exists(zipPath)) {
			string oldZipPath = zipPath + "-old";
			if (System.IO.File.Exists(oldZipPath)) System.IO.File.Delete(oldZipPath);
			System.IO.File.Move(zipPath, oldZipPath);
		}
		
		ZipFile.CreateFromDirectory(backupFolderPath, zipPath);
		return new FileContentResult(System.IO.File.ReadAllBytes(zipPath), "application/zip");
	}
}