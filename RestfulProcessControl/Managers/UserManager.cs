using RestfulProcessControl.Models;
using RestfulProcessControl.Util;

namespace RestfulProcessControl.Managers;

public static class UserManager
{
	/// <summary>
	/// Gets all users
	/// </summary>
	/// <returns>A Collection of user models (password will always be null)</returns>
	public static IEnumerable<UserModel>? GetAllUsers()
	{
		using var connection = new DatabaseConnection(Globals.ConnectionString);
		if (!connection.Get().AddTable("user").AddColumn("username").AddColumn("role")
				.TryExecute(out var elementList)) return null;
		List<UserModel> users = new();
		for (var i = 0; i < elementList["username"].Count; i++)
			users.Add(new UserModel((string)elementList["username"][i], (string)elementList["role"][i]));
		return users;
	}

	/// <summary>
	/// Creates a user in the database from a UserModel
	/// </summary>
	/// <param name="user">The CreateUserModel to create the user from</param>
	/// <returns>true if the user was created, false otherwise</returns>
	public static bool CreateUser(CreateUserModel user) => user.Username is not null && user.Password is not null &&
														   user.Role is not null && CreateUser(user.Username,
															   user.Password, user.Role);

	/// <summary>
	/// Creates a user in the database from the specified parameters
	/// </summary>
	/// <param name="username">The username for the user</param>
	/// <param name="password">The unencrypted password for the user</param>
	/// <param name="role">The role for the user</param>
	/// <returns>true if the user was created, false otherwise</returns>
	public static bool CreateUser(string username, string password, string role)
	{
		using var connection = new DatabaseConnection(Globals.ConnectionString);
		return connection.Insert().SetTable("user").AddParameter("username", username)
			.AddParameter("password", BCrypt.Net.BCrypt.HashPassword(password)).AddParameter("role", role)
			.TryExecute();
	}

	/// <summary>
	/// Deletes a user from the database
	/// </summary>
	/// <param name="user">The user to delete from the database (role ignored if null, password always ignored)</param>
	/// <returns>if the deletion was successful</returns>
	public static bool DeleteUser(UserModel user) => user.Username is not null && DeleteUser(user.Username, user.Role);

	/// <summary>
	/// Deletes a user from the database
	/// </summary>
	/// <param name="username">The username of the user</param>
	/// <returns></returns>
	public static bool DeleteUser(string username) => DeleteUser(username, null);

	/// <summary>
	/// Deletes a user from the database
	/// </summary>
	/// <param name="username">The username of the user</param>
	/// <param name="role">The role of the user (ignored if null)</param>
	/// <returns></returns>
	public static bool DeleteUser(string username, string? role)
	{
		using var connection = new DatabaseConnection(Globals.ConnectionString);
		return role is not null
			? connection.Delete().SetTable("user").IfEqual("username", username).IfEqual("role", role).TryExecute()
			: connection.Delete().SetTable("user").IfEqual("username", username).TryExecute();
	}

	/// <summary>
	/// Checks if the given user exists (password will be ignored) (role will be ignored if null)
	/// </summary>
	/// <param name="user">The user to check</param>
	/// <returns>true if the user exists, false otherwise</returns>
	public static bool HasUser(UserModel user) => user.Username is not null && HasUser(user.Username, user.Role);

	/// <summary>
	/// Checks if the given user exists
	/// </summary>
	/// <param name="username">The username of the user to find</param>
	/// <returns>true if the user exists, false otherwise</returns>
	public static bool HasUser(string username) => HasUser(username, null);

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
	/// <param name="username">The username of the user to get</param>
	/// <returns>A UserModel containing the retrieved information, or null if user could not be found</returns>
	public static UserModel? GetUser(string username) => GetUser(username, null);

	/// <summary>
	/// Gets a user from the database
	/// </summary>
	/// <param name="user">The user to get</param>
	/// <returns>A UserModel containing the retrieved information, or null if user could not be found</returns>
	public static UserModel? GetUser(UserModel user) => user.Username is not null ? GetUser(user.Username, user.Role) : null;

	/// <summary>
	/// Gets a user from the database
	/// </summary>
	/// <param name="username">The username of the user to get</param>
	/// <param name="role">The role of the user to get (ignored if null)</param>
	/// <returns>A UserModel containing the retrieved information, or null if user could not be found</returns>
	public static UserModel? GetUser(string username, string? role)
	{
		using var db = new DatabaseConnection(Globals.ConnectionString);
		Dictionary<string, List<object>> elementList;
		var success = role is not null
			? db.Get().AddTable("user").AddColumn("username").AddColumn("role").IfEqual("username", username)
				.IfEqual("role", role).TryExecute(out elementList)
			: db.Get().AddTable("user").AddColumn("username").AddColumn("role").IfEqual("username", username)
				.TryExecute(out elementList);
		if (!success || elementList["username"].Count < 1) return null;
		return new UserModel((string)elementList["username"][0], (string)elementList["role"][0]);
	}

	/// <summary>
	/// Checks if a certain password fits to a certain username
	/// </summary>
	/// <param name="username">The username to check for</param>
	/// <param name="password">The password to check for</param>
	/// <returns>true if the password authenticates the user, false otherwise</returns>
	public static bool CheckPassword(string username, string password)
	{
		using DatabaseConnection db = new(Globals.ConnectionString);
		if (!db.Get().AddColumn("password").AddTable("user").IfEqual("username", username)
				.TryExecute(out var elementList)) return false;
		if (elementList["password"].Count < 1) return false;
		var pwHash = (string)elementList["password"][0];
		return BCrypt.Net.BCrypt.Verify(password, pwHash);
	}

	/// <summary>
	/// Changes a users password
	/// </summary>
	/// <param name="user">The user information necessary to change the password</param>
	/// <returns>true if the action was successful, false otherwise</returns>
	public static bool ChangePassword(EditPasswordUserModel user) => user.Username is not null &&
																		user.PasswordOld is not null &&
																		user.PasswordNew is not null &&
																		ChangePassword(user.Username, user.PasswordOld,
																			user.PasswordNew);

	/// <summary>
	/// Changes a users password
	/// </summary>
	/// <param name="username">The username of the user whose password to edit</param>
	/// <param name="oldPassword">The old password of the user</param>
	/// <param name="newPassword">The new password to replace the old</param>
	/// <returns>true if the operation was successful, false otherwise</returns>
	public static bool ChangePassword(string username, string oldPassword, string newPassword)
	{
		if (!CheckPassword(username, oldPassword)) return false;
		var pwHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
		using var db = new DatabaseConnection(Globals.ConnectionString);
		return db.Edit().SetTable("user").IfEqual("username", username)
			.AddEdit("password", pwHash).TryExecute();
	}
}