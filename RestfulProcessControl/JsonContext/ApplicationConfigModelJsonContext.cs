using RestfulProcessControl.Models;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.JsonContext;

[JsonSerializable(typeof(ApplicationConfigModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class ApplicationConfigModelJsonContext : JsonSerializerContext
{ }