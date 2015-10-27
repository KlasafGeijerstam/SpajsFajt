using System;

namespace SpajsFajt
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                using (var game = new Game1("127.0.0.1", 5050, "testBot", true))
                    game.Run();
            }
            else
            {
                using (var game = new Game1(args[0], int.Parse(args[1]), args[2], bool.Parse(args[3])))
                    game.Run();
            }
        }
    }
#endif
}
