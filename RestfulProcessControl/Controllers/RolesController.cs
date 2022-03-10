using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Models;

namespace RestfulProcessControl.Controllers;

[Route("Role")]
[ApiController]
public class RolesController : ControllerBase
{
	/// <summary>
	/// Gets a role from it's identifier
	/// </summary>
	/// <param name="role">The identifier of the role to get</param>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <returns>A RoleModel containing information about the role</returns>
	[RequireHttps]
	[HttpGet("{role}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleModel))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetRole([FromRoute] string role, [FromQuery] string jwt)
	{
		if (!Authenticator.IsTokenValid(jwt, out _)) return Forbid();
		var rm = RoleModel.FromName(role);
		return rm is null ? NotFound() : Ok(rm);
	}
}