using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoPhysics.Core.Configuration
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppConfig))]
    internal partial class AppConfigContext : JsonSerializerContext
    { }

    public class AppConfig
    {
        public int Width { get; set; } = 1280;
        public int Height { get; set; } = 720;
        public bool Fullscreen { get; set; } = false;

        public int LogFilesToKeep { get; set; } = 5;

        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appconfig.json");
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, TypeInfoResolver = AppConfigContext.Default };

        public static AppConfig Load()
        {
            if (!File.Exists(ConfigPath))
            {
                var defaultConfig = new AppConfig();
                defaultConfig.Save();
                return defaultConfig;
            }

            var jsonString = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<AppConfig>(jsonString, Options) ?? new AppConfig();
        }

        public void Save()
        {
            var jsonString = JsonSerializer.Serialize(this, Options);
            File.WriteAllText(ConfigPath, jsonString);
        }
    }
}