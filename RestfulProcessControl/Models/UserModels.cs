using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.Models;

public class UserModel
{
	[JsonInclude]
	public string? Username { get; set; }
	[JsonInclude]
	public string? Role { get; set; }

	public UserModel()
	{
		Username = null;
		Role = null;
	}

	public UserModel(string? username, string? role)
	{
		Username = username;
		Role = role;
	}

	public static implicit operator LoginUserModel(UserModel user) => new(user.Username, null);

	public string Serialize() => Serialize(this);

	public static string Serialize(UserModel data) =>
		JsonSerializer.Serialize(data, UserModelJsonContext.Default.UserModel);

	public static UserModel? Deserialize(string data)
	{
		try { return JsonSerializer.Deserialize(data, UserModelJsonContext.Default.UserModel); }
		catch { return null; }
	}
}

public class LoginUserModel
{
	[JsonInclude]
	public string? Username { get; set; }
	[JsonInclude]
	public string? Password { get; set; }

	public LoginUserModel()
	{
		Username = null;
		Password = null;
	}

	public LoginUserModel(string? username, string? password)
	{
		Username = username;
		Password = password;
	}

	public static implicit operator UserModel(LoginUserModel user) =>
		user.Username is null
			? new UserModel(user.Username, null)
			: UserManager.GetUser(user.Username, null) ?? new UserModel(user.Username, null);

	public string Serialize() => Serialize(this);

	public static string Serialize(LoginUserModel data) =>
		JsonSerializer.Serialize(data, LoginUserModelJsonContext.Default.LoginUserModel);

	public static LoginUserModel? Deserialize(string data)
	{
		try { return JsonSerializer.Deserialize(data, LoginUserModelJsonContext.Default.LoginUserModel); }
		catch { return null; }
	}
}

public class CreateUserModel
{
	[JsonInclude]
	public string? Username { get; set; }
	[JsonInclude]
	public string? Password { get; set; }
	[JsonInclude]
	public string? Role { get; set; }

	public CreateUserModel()
	{
		Username = null;
		Password = null;
		Role = null;
	}

	public CreateUserModel(string? username, string? password, string? role)
	{
		Username = username;
		Password = password;
		Role = role;
	}

	public static implicit operator UserModel(CreateUserModel user) => new(user.Username, user.Role);

	public static implicit operator LoginUserModel(CreateUserModel user) => new(user.Username, user.Password);

	public string Serialize() => Serialize(this);
	public static string Serialize(CreateUserModel data) =>
		JsonSerializer.Serialize(data, CreateUserModelJsonContext.Default.CreateUserModel);

	public static CreateUserModel? Deserialize(string data)
	{
		try { return JsonSerializer.Deserialize(data, CreateUserModelJsonContext.Default.CreateUserModel); }
		catch { return null; }
	}
}

public class EditPasswordUserModel
{
	[JsonInclude]
	public string? Username { get; set; }
	[JsonInclude]
	public string? PasswordOld { get; set; }
	[JsonInclude]
	public string? PasswordNew { get; set; }

	public EditPasswordUserModel()
	{
		Username = null;
		PasswordOld = null;
		PasswordNew = null;
	}

	public EditPasswordUserModel(string? username, string? passwordOld, string? passwordNew)
	{
		Username = username;
		PasswordOld = passwordOld;
		PasswordNew = passwordNew;
	}

	public static implicit operator UserModel(EditPasswordUserModel user) => user.Username is null
		? new UserModel(user.Username, null)
		: UserManager.GetUser(user.Username, null) ?? new UserModel(user.Username, null);

	public static implicit operator LoginUserModel(EditPasswordUserModel user) => new(user.Username, user.PasswordOld);

	public string Serialize() => Serialize(this);
	public static string Serialize(EditPasswordUserModel data) =>
		JsonSerializer.Serialize(data, EditPasswordUserModelJsonContext.Default.EditPasswordUserModel);

	public static EditPasswordUserModel? Deserialize(string data)
	{
		try { return JsonSerializer.Deserialize(data, EditPasswordUserModelJsonContext.Default.EditPasswordUserModel); }
		catch { return null; }
	}
}

[JsonSerializable(typeof(UserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class UserModelJsonContext : JsonSerializerContext
{ }

[JsonSerializable(typeof(LoginUserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class LoginUserModelJsonContext : JsonSerializerContext
{ }

[JsonSerializable(typeof(CreateUserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CreateUserModelJsonContext : JsonSerializerContext
{ }

[JsonSerializable(typeof(EditPasswordUserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class EditPasswordUserModelJsonContext : JsonSerializerContext
{ }