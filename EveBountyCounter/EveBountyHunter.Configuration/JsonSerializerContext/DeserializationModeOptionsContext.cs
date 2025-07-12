using System.Text.Json.Serialization;
using EveBountyHunter.Configuration.Models;

namespace EveBountyHunter.Configuration.JsonSerializerContext;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(EveBountyCounterConfiguration))]
internal partial class DeserializationModeOptionsContext : System.Text.Json.Serialization.JsonSerializerContext
{
}