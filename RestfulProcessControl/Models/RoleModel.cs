using System.Text.Json.Serialization;

namespace RestfulProcessControl.Models;

public class RoleModel
{
	[JsonInclude]
	public string? Name { get; set; }
	[JsonInclude]
	public int Permissions { get; set; }

	public RoleModel()
	{
		Name = null;
		Permissions = 0;
	}

	public RoleModel(string name, int permissions)
	{
		Name = name;
		Permissions = permissions;
	}

	public static RoleModel? FromName(string name)
	{
		return null;
	}

	public bool HasPermission(string permissionName) =>
		Enum.TryParse<PermissionId>(permissionName, out var permission) && HasPermission(permission);

	public bool HasPermission(PermissionId permission) => ((Permissions >> (int) permission) & 1) == 1;
}

public enum PermissionId
{
	AccessApps,
	ControlApps,
	AccessUsers
}