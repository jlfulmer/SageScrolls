using System;

namespace sagescroll
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SageGame game = new SageGame())
            {
                game.Run();
            }
        }
    }
#endif
}

