using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.Models;

public class UserModel
{
	public string? Username { get; set; }
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

	public static implicit operator LoginModel(UserModel user) => new(user.Username, null);

	public string Serialize() => Serialize(this);
	public static string Serialize(UserModel data) => JsonSerializer.Serialize(data, UserModelJsonContext.Default.UserModel);

	public static UserModel? Deserialize(string data)
	{
		try { return JsonSerializer.Deserialize(data, UserModelJsonContext.Default.UserModel); }
		catch { return null; }
	}
}

[JsonSerializable(typeof(UserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class UserModelJsonContext : JsonSerializerContext { }