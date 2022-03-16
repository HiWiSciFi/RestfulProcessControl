using RestfulProcessControl.Models;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.JsonContext;

[JsonSerializable(typeof(LoginUserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class LoginUserModelJsonContext : JsonSerializerContext
{ }