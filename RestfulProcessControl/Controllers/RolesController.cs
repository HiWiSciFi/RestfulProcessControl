using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Managers;
using RestfulProcessControl.Models;

namespace RestfulProcessControl.Controllers;

[Route("Roles")]
[ApiController]
public class RolesController : ControllerBase
{
	/// <summary>
	/// Gets all roles
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <returns>A List of RoleModels</returns>
	[RequireHttps]
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RoleModel>))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetRoles([FromQuery] string jwt)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.GetRole))
			return Forbid();
		return Ok(RoleManager.GetRoles());
	}

	/// <summary>
	/// Gets a role from it's identifier
	/// </summary>
	/// <param name="role">The identifier of the role to get</param>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <returns>A RoleModel containing information about the role</returns>
	[RequireHttps]
	[HttpGet("Role/{role}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleModel))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetRole([FromQuery] string jwt, [FromRoute] string role)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var jrole) || !jrole.HasPermission(PermissionId.GetRole))
			return Forbid();
		var rm = RoleManager.GetRole(role);
		return rm is null ? NotFound() : Ok(rm);
	}

	/*[RequireHttps]
	[HttpGet("Role/{role}/Members")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserModel>))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetMembers([FromQuery] string jwt, [FromRoute] string role)
	{
		return Ok();
	}*/

	/// <summary>
	/// Gets a user by their role and username
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="username">The username of the user</param>
	/// <param name="role">The role of the user</param>
	/// <returns>A UserModel containing the information of the requested user</returns>
	[RequireHttps]
	[HttpGet("Role/{role}/Members/Member/{username}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetUserByUsername([FromQuery] string jwt, [FromRoute] string username, [FromRoute] string role)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var jrole) || !jrole.HasPermission(PermissionId.GetRole) ||
		    !jrole.HasPermission(PermissionId.GetUser)) return Forbid();
		var user = UserManager.GetUser(username, role);
		return user is not null ? Ok(user) : NotFound();
	}

	/// <summary>
	/// Creates a role
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="role">The name of the role to add</param>
	/// <param name="roleModel">The RoleModel describing the role</param>
	/// <returns>nothing</returns>
	[RequireHttps]
	[HttpPost("Role")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult CreateRole([FromQuery] string jwt, [FromBody] RoleModel roleModel)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var jrole) || !jrole.HasPermission(PermissionId.CreateRole) ||
		    RoleManager.HasMorePermissions(roleModel, jrole)) return Forbid();
		return RoleManager.CreateRole(roleModel)
			? Created(
				new Uri(Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority) + "/Roles/Role/" + roleModel.Name,
				RoleManager.GetRole(roleModel.Name!))
			: Forbid();
	}

	/// <summary>
	/// Deletes a role
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="role">The name of the role to delete</param>
	/// <returns>nothing</returns>
	[RequireHttps]
	[HttpDelete("Role/{role}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult DeleteRole([FromQuery] string jwt, [FromRoute] string role)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var jrole) || !jrole.HasPermission(PermissionId.DeleteRole))
			return Forbid();
		if (RoleManager.HasMorePermissions(RoleManager.GetRole(role), jrole)) return Forbid();
		return RoleManager.DeleteRole(role) ? Ok() : NotFound();
	}

	/// <summary>
	/// Edits the permissions of a role
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="role">The name of the role to edit</param>
	/// <param name="permissions">the new value for the permissions field</param>
	/// <returns>nothing</returns>
	[RequireHttps]
	[HttpPatch("Role/{role}/Permissions")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult SetRolePermissions([FromQuery] string jwt, [FromRoute] string role,
		[FromBody] long permissions)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var jrole) || !jrole.HasPermission(PermissionId.EditRole))
			return Forbid();
		if (RoleManager.HasMorePermissions(RoleManager.GetRole(role), jrole) ||
		    RoleManager.HasMorePermissions(permissions, jrole)) return Forbid();
		return RoleManager.SetRolePermissions(role, permissions) ? Ok() : NotFound();
	}
}