using Microsoft.Data.Sqlite;
using RestfulProcessControl.Models;

namespace RestfulProcessControl;

public static class UserManager
{
	private const string ConnectionString = @"Data Source=.\users.db";

	/// <summary>
	/// Gets all users
	/// </summary>
	/// <returns>A Collection of user models (password will always be null)</returns>
	public static IEnumerable<UserModel>? GetAllUsers()
	{
		try
		{
			List<UserModel> users = new();

			using var connection = new SqliteConnection(ConnectionString);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @"SELECT username, role FROM user";
			using var reader = command.ExecuteReader();
			while (reader.Read())
			{
				users.Add(
					new UserModel(reader.GetString(reader.GetOrdinal("username")),
						reader.GetString(reader.GetOrdinal("role"))));
			}
			return users;
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Creates a user in the database from the specified parameters
	/// </summary>
	/// <param name="username">The username for the user</param>
	/// <param name="password">The unencrypted password for the user</param>
	/// <param name="role">The role for the user</param>
	/// <returns>true if the user was created, false otherwise</returns>
	public static bool CreateUser(string username, string password, string role) =>
		CreateUser(new LoginModel(username, password), role);

	/// <summary>
	/// Creates a user in the database from a UserModel
	/// </summary>
	/// <param name="user">The LoginModel to create the user from</param>
	/// <param name="role">The role for the created user</param>
	/// <returns>true if the user was created, false otherwise</returns>
	public static bool CreateUser(in LoginModel user, string role)
	{
		try
		{
			using var connection = new SqliteConnection(ConnectionString);
			connection.Open();

			var command = connection.CreateCommand();
			command.CommandText = @"INSERT INTO user (username, password, role)
								VALUES ($username, $password, $role)";
			command.Parameters.AddWithValue("$username", user.Username);
			command.Parameters.AddWithValue("$password", BCrypt.Net.BCrypt.HashPassword(user.Password));
			command.Parameters.AddWithValue("$role", role);
			return command.ExecuteNonQuery() == 1;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Deletes a user from the database
	/// </summary>
	/// <param name="user">The user to delete from the database (role ignored if null, password always ignored)</param>
	/// <returns>if the deletion was successful</returns>
	public static bool DeleteUser(in UserModel user) => user.Username is not null && DeleteUser(user.Username, user.Role);

	/// <summary>
	/// Deletes a user from the database
	/// </summary>
	/// <param name="username">The username of the user</param>
	/// <param name="role">The role of the user (ignored if null)</param>
	/// <returns></returns>
	public static bool DeleteUser(string username, string? role)
	{
		try
		{
			using var connection = new SqliteConnection(ConnectionString);
			connection.Open();

			var command = connection.CreateCommand();
			if (role is not null)
			{
				command.CommandText = @"DELETE FROM user WHERE username = $username AND role = $role";
				command.Parameters.AddWithValue("$role", role);
			}
			else
			{
				command.CommandText = @"DELETE FROM user WHERE username = $username";
			}

			command.Parameters.AddWithValue("$username", username);
			return command.ExecuteNonQuery() == 1;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Checks if the given user exists (password will be ignored) (role will be ignored if null)
	/// </summary>
	/// <param name="user">The user to check</param>
	/// <returns>true if the user exists, false otherwise</returns>
	public static bool HasUser(in UserModel user) => user.Username is not null && HasUser(user.Username, user.Role);

	/// <summary>
	/// Checks if the given user exists
	/// </summary>
	/// <param name="username">The username of the user to find</param>
	/// <param name="role">The role of the user to find (will be ignored if null)</param>
	/// <returns>true if the user exists, false otherwise</returns>
	public static bool HasUser(string username, string? role) => GetUser(username, role) is not null;

	/// <summary>
	/// Gets a user from the database
	/// </summary>
	/// <param name="user">The user to get</param>
	/// <returns>A UserModel containing the retrieved information, or null if user could not be found</returns>
	public static UserModel? GetUser(in UserModel user) => user.Username is not null ? GetUser(user.Username, user.Role) : null;

	/// <summary>
	/// Gets a user from the database
	/// </summary>
	/// <param name="username">The username of the user to get</param>
	/// <param name="role">The role of the user to get (ignored if null)</param>
	/// <returns>A UserModel containing the retrieved information, or null if user could not be found</returns>
	public static UserModel? GetUser(string username, string? role)
	{
		try
		{
			using var connection = new SqliteConnection(ConnectionString);
			connection.Open();
			using var command = connection.CreateCommand();
			if (role is not null)
			{
				command.CommandText = @"SELECT username, role FROM user WHERE username = $username AND role = $role";
				command.Parameters.AddWithValue("$role", role);
			}
			else
			{
				command.CommandText = @"SELECT username, role FROM user WHERE username = $username";
			}
			command.Parameters.AddWithValue("$username", username);
			using var reader = command.ExecuteReader();
			if (!reader.Read()) return null;
			return new UserModel(
				reader.GetString(reader.GetOrdinal("username")),
				reader.GetString(reader.GetOrdinal("role")));
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Checks if a certain password fits to a certain username
	/// </summary>
	/// <param name="username">The username to check for</param>
	/// <param name="password">The password to check for</param>
	/// <returns>true if the password authenticates the user, false otherwise</returns>
	public static bool CheckPassword(string username, string password)
	{
		try
		{
			using var connection = new SqliteConnection(ConnectionString);
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText = @"SELECT password FROM user WHERE username = $username";
			command.Parameters.AddWithValue("$username", username);
			using var reader = command.ExecuteReader();
			if (!reader.Read()) return false;
			var pwhash = reader.GetString(reader.GetOrdinal("password"));
			return BCrypt.Net.BCrypt.Verify(password, pwhash);
		}
		catch
		{
			return false;
		}
	}
}