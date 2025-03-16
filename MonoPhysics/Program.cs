using MonoPhysics.Core;

internal class Program
{
#if DEBUG
    public static Logger _log = new Logger("log.txt", true);
#else
    public static Logger _log = new Logger("log.txt", false);
#endif

    private static void Main(string[] args)
    {
        using var game = new MonoPhysics.Physics();
        game.Run();
    }
}