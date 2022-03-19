using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Models;

namespace RestfulProcessControl.Controllers;

[Route("Users")]
[ApiController]
public class UsersController : ControllerBase
{
	/// <summary>
	/// Gets all users from the database
	/// </summary>
	/// <param name="jwt">The JWT to authenticate the request</param>
	/// <returns>A collection of UserModels containing all existent users</returns>
	[RequireHttps]
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserModel>))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetAllUsers([FromQuery] string jwt)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.GetUser))
			return Forbid();
		var users = UserManager.GetAllUsers();
		return users is null ? Forbid() : Ok(users);
	}

	/// <summary>
	/// Creates a user
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="user">The user information for the new user</param>
	/// <returns>nothing</returns>
	[RequireHttps]
	[HttpPost("User")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult CreateUser([FromQuery] string jwt, [FromBody] CreateUserModel user)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.CreateUser) ||
		    user.Role is null) return Forbid();
		var r = RoleManager.GetRole(user.Role);
		if (RoleManager.HasMorePermissions(r, role)) return Forbid();
		if (r is null) return NotFound();
		return UserManager.CreateUser(user)
			? Created(
				new Uri(Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority) + "/Users/User/" + user.Username,
				UserManager.GetUser(user.Username!))
			: Forbid();
	}

	/// <summary>
	/// Deletes a user from the database
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="username">The username of the user to delete</param>
	/// <returns>nothing</returns>
	[RequireHttps]
	[HttpDelete("User/{username}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult DeleteUser([FromQuery] string jwt, [FromRoute] string username)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.DeleteUser))
			return Forbid();
		var u = UserManager.GetUser(username);
		if (u is not null && RoleManager.HasMorePermissions(RoleManager.GetRole(u.Role!), role)) return Forbid();
		return UserManager.DeleteUser(username) ? Ok() : NotFound();
	}

	/// <summary>
	/// Edits a users password
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="user">The user information of the user</param>
	/// <param name="username">The username of the user whose password to change</param>
	/// <returns>nothing</returns>
	[RequireHttps]
	[HttpPatch("User/{username}/Edit/Password")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult EditPassword([FromQuery] string jwt, [FromBody] EditPasswordUserModel user,
		[FromRoute] string username)
	{
		if (user.Username != username || !AuthenticationManager.IsTokenValid(jwt, out var role) ||
		    !role.HasPermission(PermissionId.EditUser)) return Forbid();
		var u = UserManager.GetUser(user.Username);
		if (u is not null && RoleManager.HasMorePermissions(RoleManager.GetRole(u.Role!), role)) return Forbid();
		return UserManager.ChangePassword(user) ? Ok() : NotFound();
	}

	/// <summary>
	/// Gets a user by their username
	/// </summary>
	/// <param name="jwt">The JWT to authorize the request</param>
	/// <param name="username">The username of the user</param>
	/// <returns>A UserModel containing the information of the requested user</returns>
	[RequireHttps]
	[HttpGet("User/{username}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetUser([FromQuery] string jwt, [FromRoute] string username)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out var role) || !role.HasPermission(PermissionId.GetUser))
			return Forbid();
		var user = UserManager.GetUser(username);
		return user is not null ? Ok(user) : NotFound();
	}
}