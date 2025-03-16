internal class Program
{
    private static void Main(string[] args)
    {
        using var game = new MonoPhysics.Physics();
        game.Run();
    }
}