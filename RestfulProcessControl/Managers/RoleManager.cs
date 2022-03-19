using RestfulProcessControl.Models;
using RestfulProcessControl.Util;

namespace RestfulProcessControl.Managers;

public static class RoleManager
{
	/// <summary>
	/// Gets all roles
	/// </summary>
	/// <returns>A list of RoleModels</returns>
	public static IEnumerable<RoleModel> GetRoles()
	{
		using var db = new DatabaseConnection(Globals.ConnectionString);
		var roleList = new List<RoleModel>();
		if (!db.Get().AddTable("role").AddColumn("name").AddColumn("permissions")
				.TryExecute(out var elementList)) return roleList;
		for (var i = 0; i < elementList["name"].Count; i++)
			roleList.Add(new RoleModel((string)elementList["name"][i], (long)elementList["permissions"][i]));
		return roleList;
	}

	/// <summary>
	/// Gets a role by its name
	/// </summary>
	/// <param name="roleName">The name of the role</param>
	/// <returns>a RoleModel of the role, null if the role could not be found</returns>
	public static RoleModel? GetRole(string roleName)
	{
		using var db = new DatabaseConnection(Globals.ConnectionString);
		if (!db.Get().AddTable("role").AddColumn("name").AddColumn("permissions").IfEqual("name", roleName)
				.TryExecute(out var elementList)) return null;
		return elementList["name"].Count > 0
			? new RoleModel((string)elementList["name"][0], (long)elementList["permissions"][0])
			: null;
	}

	/// <summary>
	/// Creates a role
	/// </summary>
	/// <param name="role">The RoleModel of the role to create</param>
	/// <returns>true if creation was successful, false otherwise</returns>
	public static bool CreateRole(RoleModel role) => role.Name is not null && CreateRole(role.Name, role.Permissions);

	/// <summary>
	/// Creates a role
	/// </summary>
	/// <param name="name">The name of the role to create</param>
	/// <param name="permissions">The permissions for the role</param>
	/// <returns>true if creation was successful, false otherwise</returns>
	public static bool CreateRole(string name, long permissions)
	{
		using var db = new DatabaseConnection(Globals.ConnectionString);
		return db.Insert().SetTable("role").AddParameter("name", name).AddParameter("permissions", permissions)
			.TryExecute();
	}

	/// <summary>
	/// Deletes a role
	/// </summary>
	/// <param name="name">The name of the role to delete</param>
	/// <returns>true if deletion was successful, false otherwise</returns>
	public static bool DeleteRole(string name)
	{
		using var db = new DatabaseConnection(Globals.ConnectionString);
		return db.Delete().SetTable("role").IfEqual("name", name).TryExecute();
	}

	/// <summary>
	/// Sets the permissions for a role
	/// </summary>
	/// <param name="roleName">The name of the role</param>
	/// <param name="permissions">The new permissions for the role</param>
	/// <returns>true if editing the permissions was successful, false otherwise</returns>
	public static bool SetRolePermissions(string roleName, long permissions)
	{
		using var db = new DatabaseConnection(Globals.ConnectionString);
		return db.Edit().SetTable("role").IfEqual("name", roleName).AddEdit("permissions", permissions).TryExecute();
	}

	/// <summary>
	/// Checks if perms has one or more permissions which than does not have
	/// </summary>
	/// <param name="perms">Permission value 1</param>
	/// <param name="than">Permission value 2</param>
	/// <returns>true if it does, false otherwise</returns>
	public static bool HasMorePermissions(RoleModel? perms, long than) =>
		perms is not null && HasMorePermissions(perms.Permissions, than);

	/// <summary>
	/// Checks if perms has one or more permissions which than does not have
	/// </summary>
	/// <param name="perms">Permission value 1</param>
	/// <param name="than">Permission value 2</param>
	/// <returns>true if it does, false otherwise</returns>
	public static bool HasMorePermissions(long perms, RoleModel? than) =>
		than is not null && HasMorePermissions(perms, than.Permissions);

	/// <summary>
	/// Checks if perms has one or more permissions which than does not have
	/// </summary>
	/// <param name="perms">Permission value 1</param>
	/// <param name="than">Permission value 2</param>
	/// <returns>true if it does, false otherwise</returns>
	public static bool HasMorePermissions(RoleModel? perms, RoleModel? than)
	{
		if (perms is null || than is null) return false;
		return HasMorePermissions(perms.Permissions, than.Permissions);
	}

	/// <summary>
	/// Checks if perms has one or more permissions which than does not have
	/// </summary>
	/// <param name="perms">Permission value 1</param>
	/// <param name="than">Permission value 2</param>
	/// <returns>true if it does, false otherwise</returns>
	public static bool HasMorePermissions(long perms, long than)
	{
		for (var i = 0; i < 64; i++)
			if (((perms >> i) & 1) > ((than >> i) & 1))
				return true;
		return false;
	}

	/// <summary>
	/// Gets all users who are members of a certain role
	/// </summary>
	/// <param name="rolename">The name of the role</param>
	/// <returns>A List of UserModels containing information about the members</returns>
	public static IEnumerable<UserModel> GetMembers(string rolename)
	{
		var users = new List<UserModel>();
		using var db = new DatabaseConnection(Globals.ConnectionString);
		if (!db.Get().AddTable("user").AddColumn("username").IfEqual("role", rolename)
			    .TryExecute(out var elementList)) return users;
		for (var i = 0; i < elementList["username"].Count; i++)
			users.Add(new UserModel((string) elementList["username"][i], rolename));
		return users;
	}

	/// <summary>
	/// Gets a member of a role
	/// </summary>
	/// <param name="rolename">The name of the role</param>
	/// <param name="username">The name of the user</param>
	/// <returns>A UserModel containing information about the user</returns>
	public static UserModel? GetMember(string rolename, string username) => UserManager.GetUser(username, rolename);
}