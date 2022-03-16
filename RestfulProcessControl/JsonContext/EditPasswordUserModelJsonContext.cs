using RestfulProcessControl.Models;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.JsonContext;

[JsonSerializable(typeof(EditPasswordUserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class EditPasswordUserModelJsonContext : JsonSerializerContext
{ }