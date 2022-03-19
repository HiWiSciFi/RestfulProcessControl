using System.Text.Json.Serialization;
using RestfulProcessControl.Managers;

namespace RestfulProcessControl.Models;

public class RoleModel
{
	[JsonInclude]
	public string? Name { get; set; }
	[JsonInclude]
	public long Permissions { get; }

	public RoleModel()
	{
		Name = null;
		Permissions = 0;
	}

	public RoleModel(string name, long permissions)
	{
		Name = name;
		Permissions = permissions;
	}

	public bool HasPermission(string permissionName) =>
		Enum.TryParse<PermissionId>(permissionName, out var permission) && HasPermission(permission);

	public bool HasPermission(PermissionId permission) => ((Permissions >> (int)permission) & 1) != 0;

	public static bool HasPermission(string roleName, PermissionId permission, out RoleModel? role)
	{
		role = RoleManager.GetRole(roleName);
		return role?.HasPermission(permission) == true;
	}
}