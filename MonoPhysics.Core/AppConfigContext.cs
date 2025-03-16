using System.Text.Json.Serialization;

namespace MonoPhysics.Core.Configuration
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppConfig))]
    internal partial class AppConfigContext : JsonSerializerContext
    { }
}