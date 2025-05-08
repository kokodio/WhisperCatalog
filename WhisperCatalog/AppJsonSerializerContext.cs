using System.Text.Json.Serialization;

namespace WhisperCatalog;


[JsonSerializable(typeof(string))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}