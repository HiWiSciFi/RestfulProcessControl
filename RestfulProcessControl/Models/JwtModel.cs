using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace RestfulProcessControl.Models;

public class JwtModel
{
	private static readonly byte[] SecretKey;
	static JwtModel() => SecretKey = RandomNumberGenerator.GetBytes(64);

	public class JwtPayload
	{
		[JsonPropertyName("sub")]
		public string? Subject { get; set; }
		[JsonPropertyName("role")]
		public string? Role { get; set; }
		[JsonPropertyName("iat")]
		public int IssuedAt { get; set; }
		[JsonPropertyName("exp")]
		public int ExpirationTime { get; set; }
	}

	public class JwtHeader
	{
		[JsonPropertyName("alg")]
		public string? Algorithm { get; set; }
		[JsonPropertyName("typ")]
		public string? Type { get; set; }
	}

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