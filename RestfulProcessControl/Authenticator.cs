using Microsoft.Data.Sqlite;
using RestfulProcessControl.Models;

namespace RestfulProcessControl;

public static class Authenticator
{
	/// <summary>
	/// Creates a user in the database from the specified parameters
	/// </summary>
	/// <param name="username">The username for the user</param>
	/// <param name="password">The password for the user</param>
	/// <param name="role">The role for the user</param>
	/// <returns>true if the user was created, false otherwise</returns>
	public static bool CreateUser(string username, string password, string role) =>
		CreateUser(new UserModel(username, password, role));

	/// <summary>
	/// Creates a user in the database from a UserModel
	/// </summary>
	/// <param name="user">The UserModel to create the user from</param>
	/// <returns>true if the user was created, false otherwise</returns>
	public static bool CreateUser(in UserModel user)
	{
		try
		{
			using var connection = new SqliteConnection(@"Data Source=.\users.db");
			connection.Open();

			var command = connection.CreateCommand();
			command.CommandText = @"INSERT INTO user (username, password, role)
								VALUES ($username, $password, $role)";
			command.Parameters.AddWithValue("$username", user.Username);
			command.Parameters.AddWithValue("$password", BCrypt.Net.BCrypt.HashPassword(user.Password));
			command.Parameters.AddWithValue("$role", user.Role);
			return command.ExecuteNonQuery() == 1;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Authenticates a user in the database
	/// </summary>
	/// <param name="user">The user to authenticate</param>
	/// <returns>true if the login data was correct, false otherwise</returns>
	public static bool Authenticate(in UserModel user)
	{
		try
		{
			if (user.Username is null || user.Password is null) return false;
			var pwHash = string.Empty;
			using (var connection = new SqliteConnection(@"Data Source=.\users.db"))
			{
				connection.Open();
				var command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM user WHERE username = $username";
				command.Parameters.AddWithValue("$username", user.Username);
				using var reader = command.ExecuteReader();
				if (reader.Read())
				{
					pwHash = reader.GetString(reader.GetOrdinal("password"));
					user.Role = reader.GetString(reader.GetOrdinal("role"));
				}
			}

			return BCrypt.Net.BCrypt.Verify(user.Password, pwHash);
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Checks if a JWT is valid for this application
	/// </summary>
	/// <param name="token">The JWT to check for validity</param>
	/// <returns>true, if the JWT is valid, false otherwise</returns>
	public static bool IsTokenValid(string token)
	{
		JwtModel jwt = new(token);
		return IsTokenValid(in jwt);
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
	public static JwtModel? CreateJwt(in UserModel user, int maxSessionTime)
	{
		try
		{
			JwtModel jwt = new();
			JwtModel.JwtHeader header = new();
			JwtModel.JwtPayload payload = new();

			header.Algorithm = "HS256";
			header.Type = "JWT";

			payload.Subject = user.Username;
			payload.Role = user.Role;
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