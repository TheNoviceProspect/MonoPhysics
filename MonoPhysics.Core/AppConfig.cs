using System.Text.Json;
using Microsoft.Xna.Framework.Input;

namespace MonoPhysics.Core.Configuration
{
    public class AppConfig
    {
        #region Fields

        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appconfig.json");
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, TypeInfoResolver = AppConfigContext.Default };

        #endregion Fields

        #region Properties

        public float BoundaryPadding { get; set; } = 20f; // Default value of 20 pixels
        public int BoundaryThickness { get; set; } = 2; // Default value of 2 pixels
        public bool Fullscreen { get; set; } = false;
        public int Height { get; set; }
        public Keys ImGuiConfigKey { get; set; } = Keys.F2;
        public Keys ImGuiDetailsKey { get; set; } = Keys.F9;
        public Keys ImGuiToggleKey { get; set; } = Keys.F10;
        public int LogFilesToKeep { get; set; } = 5;
        public ScreenResolution ScreenResolution { get; set; } = ScreenResolution.R900p;  // Default to 900p
        public int Width { get; set; }

        #endregion Properties

        // Add this line

        // Add this property

        // Default value of 20 pixels

        #region Public Methods

        public static AppConfig Load()
        {
            if (!File.Exists(ConfigPath))
            {
                var defaultConfig = new AppConfig();
                defaultConfig.SetResolutionDimensions();
                defaultConfig.Save();
                return defaultConfig;
            }

            var jsonString = File.ReadAllText(ConfigPath);
            var config = JsonSerializer.Deserialize<AppConfig>(jsonString, Options) ?? new AppConfig();
            config.SetResolutionDimensions();
            return config;
        }

        public static void Save(AppConfig config)
        {
            config.SetResolutionDimensions();
            string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("config.json", jsonString);
        }

        public void Save()
        {
            SetResolutionDimensions();
            var jsonString = JsonSerializer.Serialize(this, Options);
            File.WriteAllText(ConfigPath, jsonString);
        }

        public void SetResolutionDimensions()
        {
            switch (ScreenResolution)
            {
                case ScreenResolution.R720p:
                    Width = 1280;
                    Height = 720;
                    break;

                case ScreenResolution.R900p:
                    Width = 1600;
                    Height = 900;
                    break;

                case ScreenResolution.R1080p:
                    Width = 1920;
                    Height = 1080;
                    break;

                default:
                    Width = 1600;
                    Height = 900;
                    break;
            }
        }

        #endregion Public Methods
    }
}