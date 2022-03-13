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
		var rm = RoleManager.GetRole(role);
		return rm is null ? NotFound() : Ok(rm);
	}

	/// <summary>
	/// Gets a user by their role and username
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="username">The username of the user</param>
	/// <param name="role">The role of the user</param>
	/// <returns>A UserModel containing the information of the requested user</returns>
	[RequireHttps]
	[HttpGet("Role/{role}/User/{username}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetUserByUsername([FromQuery] string jwt, [FromRoute] string username, [FromRoute] string role)
	{
		if (!Authenticator.IsTokenValid(jwt, out _)) return Forbid();
		var user = UserManager.GetUser(username, role);
		return user is not null ? Ok(user) : NotFound();
	}
}