using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Models;

namespace RestfulProcessControl.Controllers;

[Route("Auth/Token")]
[ApiController]
public class AuthenticationHttpController : ControllerBase
{
	public const int MaxRefreshTime = 100; // seconds
	public const int MaxSessionTime = 20; // seconds

	private readonly ILogger<AuthenticationHttpController> _logger;
	public AuthenticationHttpController(ILogger<AuthenticationHttpController> logger) => _logger = logger;

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
		UserModel user = new("Test", null, "admin");
		return Ok(Authenticator.CreateJwt(in user, 3600)!.ToString());
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
	public IActionResult GetJwtToken([FromBody]UserModel user)
	{
		if (!Authenticator.Authenticate(in user)) return Forbid();
		var jwt = Authenticator.CreateJwt(in user, MaxSessionTime);
		if (jwt is null) return Forbid();
		Logger.Log(LogLevel.Information, "Created JWT for user {0}: {1}", user.Username, jwt.ToString());
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
	public IActionResult RefreshJwtToken([FromQuery]string jwt)
	{
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
	public IActionResult IsTokenValidRequest([FromQuery]string jwt) => Ok(Authenticator.IsTokenValid(jwt));
}
