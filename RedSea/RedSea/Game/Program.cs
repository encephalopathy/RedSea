using System;

namespace RedSeaGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RedSea game = new RedSea())
            {
                game.Run();
            }
        }
    }
#endif
}

