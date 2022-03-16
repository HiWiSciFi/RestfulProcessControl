﻿using RestfulProcessControl.Models;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.JsonContext;

[JsonSerializable(typeof(CreateUserModel))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CreateUserModelJsonContext : JsonSerializerContext
{ }