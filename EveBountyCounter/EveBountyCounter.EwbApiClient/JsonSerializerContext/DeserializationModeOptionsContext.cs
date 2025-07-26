using System.Text.Json.Serialization;
using EveBountyCounter.EwbApiClient.Contracts;

namespace EveBountyCounter.EwbApiClient.JsonSerializerContext;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(IEnumerable<CharacterResponse>))]
[JsonSerializable(typeof(RealtimeBountyUpdateResponse))]
internal partial class DeserializationModeOptionsContext : System.Text.Json.Serialization.JsonSerializerContext;