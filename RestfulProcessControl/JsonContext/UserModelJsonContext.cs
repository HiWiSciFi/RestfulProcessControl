using RestfulProcessControl.Models;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.JsonContext;

[JsonSerializable(typeof(UserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class UserModelJsonContext : JsonSerializerContext
{ }