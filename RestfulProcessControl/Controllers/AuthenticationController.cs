using Microsoft.AspNetCore.Mvc;
using RestfulProcessControl.Models;

namespace RestfulProcessControl.Controllers;

[Route("Auth/Token")]
[ApiController]
public class AuthenticationController : ControllerBase
{
	public const int MaxSessionTime = 10; // seconds

	private readonly ILogger<AuthenticationController> _logger;
	public AuthenticationController(ILogger<AuthenticationController> logger) => _logger = logger;

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
		_logger.LogInformation("Created JWT for user {}: {}", user.Username, jwt.ToString());
		return Ok(jwt.ToString());
	}

	/// <summary>
	/// Checks if a JWT is valid for this application
	/// </summary>
	/// <param name="token">The JWT to check for validity</param>
	/// <returns>true, if the JWT is valid, false otherwise</returns>
	[RequireHttps]
	[HttpGet("Valid")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
	public IActionResult IsTokenValidRequest([FromQuery]string token) => Ok(Authenticator.IsTokenValid(token));
}