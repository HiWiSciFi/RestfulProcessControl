using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Security.Cryptography;

namespace RestfulProcessControl.Controllers;

[Route("Auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
	public const int maxSessionTimeSeconds = 120;
	private const string JwtHeader = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
	private const string JwtHeaderBase64 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";

	private static readonly byte[] secretKey;

	static AuthenticationController() => secretKey = RandomNumberGenerator.GetBytes(64);

	private readonly ILogger<AuthenticationController> _logger;
	public AuthenticationController(ILogger<AuthenticationController> logger) => _logger = logger;

	[RequireHttps]
	[HttpGet("Token")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IActionResult GetJwtToken(string username, string password) {
		try
		{
			// check credentials
			// TODO: check credentials in DB

			// Generate JWT
			StringBuilder JWT = new();
			JWT.Append(JwtHeaderBase64);
			JWT.Append('.');

			StringBuilder payload = new();
			payload.Append("{\"iat\":");
			payload.Append(GetUnixTimestamp());
			payload.Append(",\"sub\":\"");
			payload.Append(username);
			payload.Append("\"}");
			JWT.Append(Base64UrlEncode(payload.ToString()));

			using (HMACSHA256 hmac = new(secretKey))
			{
				string signature = Base64UrlEncode(
						hmac.ComputeHash(
							Encoding.UTF8.GetBytes(
								JWT.ToString())));
				JWT.Append('.');
				JWT.Append(signature);
			}

			return Ok(JWT.ToString());
		} catch { return Forbid(); }
	}

	public static bool IsTokenValid(string token) => IsTokenValid(SplitJwt(token));

	public static bool IsTokenValid((string?, string?, string?) token) {
		try
		{
			if (token.Item1 is null || token.Item2 is null || token.Item3 is null) return false;
			if (token.Item1 != JwtHeaderBase64) return false;
			using (HMACSHA256 hmac = new(secretKey))
			{
				string computedSignature = Base64UrlEncode(
					hmac.ComputeHash(
						Encoding.UTF8.GetBytes(
							token.Item1 + '.' + token.Item2)));
				if (computedSignature != token.Item3) return false;
			}

			// TODO: check if token is expired

			return true;
		} catch { return false; }
	}

	private static int GetUnixTimestamp() => (int)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;

	[RequireHttps]
	[HttpGet("Token/Remaining")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetRemainingSessionTime([FromBody]string token) {
		return NotFound();
	}

	public static (string?, string?, string?) SplitJwt(string token) {
		string[] xyz = token.Split('.');
		return (xyz.Length >= 1 ? xyz[0] : null, xyz.Length >= 2 ? xyz[1] : null, xyz.Length >= 3 ? xyz[2] : null);
	}

	public static string Base64UrlEncode(string input) => Base64UrlEncode(Encoding.UTF8.GetBytes(input));
	public static string Base64UrlEncode(byte[] input) => WebEncoders.Base64UrlEncode(input);
	public static string Base64UrlDecode(string input) => Encoding.UTF8.GetString(Base64UrlDecodeBytes(input));
	public static byte[] Base64UrlDecodeBytes(string input) => WebEncoders.Base64UrlDecode(input);
}