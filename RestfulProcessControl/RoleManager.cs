using RestfulProcessControl.Models;
using RestfulProcessControl.Util;

namespace RestfulProcessControl;

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
}