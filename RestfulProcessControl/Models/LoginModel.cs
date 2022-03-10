using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.Models;

public class LoginModel
{
	[JsonInclude]
	public string? Username { get; set; }
	[JsonInclude]
	public string? Password { get; set; }

	public LoginModel()
	{
		Username = null;
		Password = null;
	}

	public LoginModel(string? username, string? password)
	{
		Username = username;
		Password = password;
	}

	public static implicit operator UserModel(LoginModel user) =>
		user.Username is null
			? new UserModel(user.Username, null)
			: UserManager.GetUser(user.Username, null) ?? new UserModel(user.Username, null);

	public string Serialize() => Serialize(this);
	public static string Serialize(LoginModel data) => JsonSerializer.Serialize(data, LoginModelJsonContext.Default.LoginModel);

	public static LoginModel? Deserialize(string data)
	{
		try { return JsonSerializer.Deserialize(data, LoginModelJsonContext.Default.LoginModel); }
		catch { return null; }
	}
}

[JsonSerializable(typeof(LoginModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class LoginModelJsonContext : JsonSerializerContext { }