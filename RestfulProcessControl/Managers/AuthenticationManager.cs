using RestfulProcessControl.Models;
using RestfulProcessControl.Util;

namespace RestfulProcessControl.Managers;

public static class AuthenticationManager
{
	/// <summary>
	/// Authenticates a user in the database
	/// </summary>
	/// <param name="user">The user to authenticate</param>
	/// <returns>true if the login data was correct, false otherwise</returns>
	public static bool Authenticate(LoginUserModel user)
	{
		Logger.LogInformation("Authenticating user \"{0}\"", user.Username);
		if (user.Username is null || user.Password is null) return false;
		return UserManager.CheckPassword(user.Username, user.Password);
	}

	/// <summary>
	/// Checks if a JWT is valid for this application
	/// </summary>
	/// <param name="token">The JWT to check for validity</param>
	/// <param name="role">The RoleModel for the role the token authorizes</param>
	/// <returns>true, if the JWT is valid, false otherwise</returns>
	public static bool IsTokenValid(string token, out RoleModel role)
	{
		var jwt = new JwtModel(token);
		role = new RoleModel();
		if (!jwt.IsValid()) return false;
		role = jwt.Payload!.Role ?? new RoleModel(jwt.Payload.RoleName ?? string.Empty, 0);
		return true;
	}

	/// <summary>
	/// Checks if a JWT is valid for this application
	/// </summary>
	/// <param name="token">The JWT to check for validity</param>
	/// <returns>true, if the JWT is valid, false otherwise</returns>
	public static bool IsTokenValid(in JwtModel token) => token.IsValid();

	/// <summary>
	/// Creates a JWT (DOESN'T AUTHENTICATE USER) for a user and a specified maximum session time (without refreshing)
	/// </summary>
	/// <param name="user">The User to create the JWT for</param>
	/// <param name="maxSessionTime">The maximum session time (without refreshing)</param>
	/// <returns>The JWT that has been created or null if it could not be created</returns>
	public static JwtModel? CreateJwt(UserModel user, int maxSessionTime)
	{
		try
		{
			JwtModel jwt = new();
			JwtModel.JwtHeader header = new();
			JwtModel.JwtPayload payload = new();

			header.Algorithm = "HS256";
			header.Type = "JWT";

			payload.Subject = user.Username;
			payload.RoleName = user.Role;
			payload.IssuedAt = UnixTime.Now;
			payload.ExpirationTime = payload.IssuedAt + maxSessionTime;

			jwt.Header = header;
			jwt.Payload = payload;

			jwt.Fill();
			jwt.GenerateSignature();

			return jwt;
		}
		catch
		{
			return null;
		}
	}
}