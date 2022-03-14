using System.Text.Json.Serialization;

namespace RestfulProcessControl.Models;

public class RoleModel
{
	[JsonInclude]
	public string? Name { get; set; }
	[JsonInclude]
	public long Permissions { get; set; }

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

	public bool HasPermission(PermissionId permission) => ((Permissions >> (int)permission) & 1) == 1;

	public static bool HasPermission(string roleName, PermissionId permission, out RoleModel? role)
	{
		role = RoleManager.GetRole(roleName);
		return role?.HasPermission(permission) == true;
	}
}