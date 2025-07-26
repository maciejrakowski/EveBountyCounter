using System.Text.Json.Serialization;
using EveBountyHunter.Configuration.Models;

namespace EveBountyHunter.Configuration.JsonSerializerContext;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(EveBountyCounterConfiguration))]
[JsonSerializable(typeof(EveWorkbenchApiKey))]
internal partial class SerializationModeOptionsContext : System.Text.Json.Serialization.JsonSerializerContext;