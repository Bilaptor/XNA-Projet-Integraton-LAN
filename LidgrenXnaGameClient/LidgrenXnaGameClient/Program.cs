
namespace XnaGameClient
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Menu1 menu = new Menu1())
            {
                menu.Run();
            }
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

