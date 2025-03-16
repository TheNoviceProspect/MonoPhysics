using MonoPhysics.Core;
using MonoPhysics.Core.Configuration;

internal class Program
{
#if DEBUG
    public static Logger _log = new Logger("log.txt", true);
#else
    public static Logger _log = new Logger("log.txt", false);
#endif

    /// <summary>
    /// Application configuration valid for the entire application's lifetime
    /// </summary>
    internal static AppConfig _config { get; private set; }

    /// <summary>
    /// Main entry point for the application
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        _log.Info("Physics Demo v0.1 started");
        _log.Info("Loading Config");
        _config = AppConfig.Load();
        _log.Info($"Config loaded: [Size:{_config.Width}x{_config.Height}] [Fullscreen:{_config.Fullscreen}] [Logs to keep: {_config.LogFilesToKeep}]");
        _log.Info("Initializing App Window");
        using var game = new MonoPhysics.Physics();
        game.Run();
        _log.Info("Physics Demo v0.1 ended");
    }
}