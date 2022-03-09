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
		if (!Authenticator.IsTokenValid(jwt)) return Forbid();
		var users = UserManager.GetAllUsers();
		return users is null ? Forbid() : Ok(users);
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
	public IActionResult GetUserByUsername([FromQuery] string jwt, [FromRoute] string username)
	{
		if (!Authenticator.IsTokenValid(jwt)) return Forbid();
		var user = UserManager.GetUser(username, null);
		return user is not null ? Ok(user) : NotFound();
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
		if (!Authenticator.IsTokenValid(jwt)) return Forbid();
		var user = UserManager.GetUser(username, role);
		return user is not null ? Ok(user) : NotFound();
	}
}