namespace RestfulProcessControl.Models;

// max. of 64 permissions
public enum PermissionId
{
	// Users [4]
	CreateUser,
	DeleteUser,
	EditUser,
	GetUser,

	// Roles [4]
	CreateRole,
	DeleteRole,
	EditRole,
	GetRole,

	// Applications []
	CreateApplication,
	DeleteApplication,
	EditApplication,
	GetApplication,

	// Backups [2]
	CreateBackup,
	DownloadBackup
}