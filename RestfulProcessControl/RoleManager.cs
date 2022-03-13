using RestfulProcessControl.Models;

namespace RestfulProcessControl;

public static class RoleManager
{
	public static RoleModel? GetRole(string roleName)
	{
		try
		{
			using var db = new DatabaseConnection(Globals.ConnectionString);
			if (!db.Get().AddTable("role").AddColumn("name").AddColumn("permissions").IfEqual("name", roleName)
				    .TryExecute(out var elementList)) return null;
			return elementList["name"].Count > 0
				? new RoleModel((string)elementList["name"][0], (long)elementList["permissions"][0])
				: null;
		}
		catch
		{
			return null;
		}
	}
}