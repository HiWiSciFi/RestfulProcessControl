using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Managers;
using RestfulProcessControl.Models;
using RestfulProcessControl.Util;

namespace RestfulProcessControl.Controllers;

[Route("Auth/Token")]
[ApiController]
public class AuthenticationController : ControllerBase
{
	public const int MaxRefreshTime = 100; // seconds
	public const int MaxSessionTime = 20; // seconds

	// TODO: REMOVE BEFORE DEPLOYMENT !!!
	/// <summary>
	/// Creates a JWT with administrator privileges and a session time of one hour
	/// </summary>
	/// <returns>The created JWT</returns>
	[RequireHttps]
	[HttpPost("TempGetAdminJwt")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
	public IActionResult TestGet()
	{
		UserModel user = new("Test", "admin");
		return Ok(AuthenticationManager.CreateJwt(user, 3600)!.ToString());
	}

	/// <summary>
	/// Authenticates a User using their login credentials and responds with a JWT for their session
	/// </summary>
	/// <param name="user">The user to login with</param>
	/// <returns>A stringified JWT for authenticating further requests</returns>
	[RequireHttps]
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetJwtToken([FromBody] LoginUserModel user)
	{
		if (!AuthenticationManager.Authenticate(user)) return Forbid();
		var uuser = (UserModel)user;
		var jwt = AuthenticationManager.CreateJwt(uuser, MaxSessionTime);
		if (jwt is null) return Forbid();
		Logger.LogInformation("Created JWT for user {0}: {1}", user.Username, jwt.ToString());
		return Ok(jwt.ToString());
	}

	/// <summary>
	/// Refreshes a JWT
	/// </summary>
	/// <param name="jwt">The JWT to refresh</param>
	/// <returns>The refreshed JWT or 403Forbidden if it is not allowed to be refreshed</returns>
	[RequireHttps]
	[HttpPost("Refresh")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult RefreshJwtToken([FromQuery] string jwt)
	{
		if (!AuthenticationManager.IsTokenValid(jwt, out _)) return Forbid();
		var token = new JwtModel(jwt);
		if (token.Refresh(MaxSessionTime, MaxRefreshTime)) return Ok(token.ToString());
		return Forbid();
	}

	/// <summary>
	/// Checks if a JWT is valid for this application
	/// </summary>
	/// <param name="jwt">The JWT to check for validity</param>
	/// <returns>true, if the JWT is valid, false otherwise</returns>
	[RequireHttps]
	[HttpGet("Valid")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
	public IActionResult IsTokenValidRequest([FromQuery] string jwt) => Ok(AuthenticationManager.IsTokenValid(jwt, out _));
}
