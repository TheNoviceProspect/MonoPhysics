using System.Text.Json;

namespace MonoPhysics.Core.Configuration
{
    public class AppConfig
    {
        #region Fields

        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appconfig.json");
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, TypeInfoResolver = AppConfigContext.Default };

        #endregion Fields

        #region Properties

        public float BoundaryPadding { get; set; } = 20f;
        public bool Fullscreen { get; set; } = false;
        public int Height { get; set; } = 720;
        public int LogFilesToKeep { get; set; } = 5;
        public int Width { get; set; } = 1280;

        #endregion Properties

        // Default value of 20 pixels

        #region Public Methods

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

        #endregion Public Methods
    }
}