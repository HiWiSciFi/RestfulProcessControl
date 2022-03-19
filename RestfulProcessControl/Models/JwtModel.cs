using Microsoft.AspNetCore.WebUtilities;
using RestfulProcessControl.Util;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestfulProcessControl.Managers;

namespace RestfulProcessControl.Models;

public class JwtModel
{
	private static readonly byte[] SecretKey;
	static JwtModel() => SecretKey = RandomNumberGenerator.GetBytes(64);

	/// <summary>
	/// Representation for the Payload of a JWT
	/// </summary>
	public class JwtPayload
	{
		[JsonPropertyName("sub")] public string? Subject { get; set; }
		[JsonPropertyName("role")] public string? RoleName { get; set; }
		[JsonIgnore] private RoleModel? RoleObject { get; set; }
		[JsonIgnore]
		public RoleModel? Role =>
			RoleName is null
				? null
				: RoleObject is null
					? RoleObject = RoleManager.GetRole(RoleName)
					: RoleObject.Name == RoleName
						? RoleObject
						: RoleObject = RoleManager.GetRole(RoleName);

		[JsonPropertyName("iat")] public int IssuedAt { get; set; }
		[JsonPropertyName("exp")] public int ExpirationTime { get; set; }
	}

	/// <summary>
	/// Representation for the Header of a JWT
	/// </summary>
	public class JwtHeader
	{
		[JsonPropertyName("alg")]
		public string? Algorithm { get; set; }
		[JsonPropertyName("typ")]
		public string? Type { get; set; }
	}

	/// <summary>
	/// The string of the full Base64Url encoded JWT
	/// </summary>
	private string? Jwt { get; set; }

	public string? EncodedHeader { get; set; }
	public JwtHeader? Header { get; set; }

	public string? EncodedPayload { get; set; }
	public JwtPayload? Payload { get; set; }

	public string? Signature { get; set; }

	public JwtModel()
	{
		Jwt = null;

		EncodedHeader = null;
		Header = null;

		EncodedPayload = null;
		Payload = null;

		Signature = null;
	}

	public JwtModel(string jwt)
	{
		Jwt = jwt;
		Fill();
	}

	public override string ToString() => Jwt ?? Fill() ?? "Invalid parameters";

	/// <summary>
	/// Fills the missing elements of the JWT
	/// </summary>
	/// <returns>The encoded JWT string if it could be created</returns>
	public string? Fill()
	{
		if (Jwt is not null)
		{
			var xyz = Jwt.Split('.');
			if (xyz.Length != 3) return null;
			EncodedHeader = xyz[0];
			EncodedPayload = xyz[1];
			Signature = xyz[2];
		}

		if (Payload is null)
		{
			if (EncodedPayload is not null)
			{
				Payload = JsonSerializer.Deserialize<JwtPayload>(
					Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(EncodedPayload)));
			}
		}
		else if (EncodedPayload is null)
		{
			EncodedPayload = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Payload)));
		}

		if (Header is null)
		{
			if (EncodedHeader is not null)
			{
				Header = JsonSerializer.Deserialize<JwtHeader>(
					Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(EncodedHeader)));
			}
		}
		else if (EncodedHeader is null)
		{
			EncodedHeader = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Header)));
		}

		if (Signature is null && !GenerateSignature()) return null;

		if (EncodedHeader is not null && EncodedPayload is not null && Signature is not null)
			Jwt = EncodedHeader + '.' + EncodedPayload + '.' + Signature;

		return Jwt;
	}

	/// <summary>
	/// Checks the validity of a JWT
	/// </summary>
	/// <returns>true if the JWT is valid, false otherwise</returns>
	public bool IsValid()
	{
		if (Signature is null || Payload is null) return false;
		var successful = false;
		try
		{
			var origSig = Signature;
			successful = GenerateSignature();
			successful = successful && origSig == Signature;
			Signature = origSig;

			if (Payload.ExpirationTime < UnixTime.Now) successful = false;
		}
		catch
		{
			// ignored
		}

		return successful;
	}

	/// <summary>
	/// Refreshes the token
	/// </summary>
	/// <param name="by">The amount of seconds to add to the expiration date</param>
	/// <param name="max">The total max time frame the token will be valid</param>
	/// <returns>true if successful, false otherwise</returns>
	public bool Refresh(int by, int max)
	{
		if (!IsValid() || Payload == null) return false;

		// no further refreshing allowed
		if (Payload.ExpirationTime - Payload.IssuedAt >= max) return false;

		var exp = UnixTime.Now + by;
		// if full length cannot be added
		if (exp - Payload.IssuedAt > max) Payload.ExpirationTime = exp - (Payload.IssuedAt + max - exp);
		// add full time otherwise
		else Payload.ExpirationTime = exp;

		// refresh necessary values
		EncodedPayload = null;
		Signature = null;
		Jwt = null;
		Fill();
		return true;
	}

	/// <summary>
	/// Generates the signature according to the stored data
	/// </summary>
	/// <returns>true if successful, false otherwise</returns>
	public bool GenerateSignature()
	{
		try
		{
			if (EncodedHeader is not null && EncodedPayload is not null)
			{
				using HMACSHA256 hmac = new(SecretKey);
				Signature = WebEncoders.Base64UrlEncode(
					hmac.ComputeHash(Encoding.ASCII.GetBytes(EncodedHeader + '.' + EncodedPayload)));
				Jwt = EncodedHeader + '.' + EncodedPayload + '.' + Signature;
				return true;
			}
		}
		catch
		{
			// ignored
		}

		return false;
	}
}