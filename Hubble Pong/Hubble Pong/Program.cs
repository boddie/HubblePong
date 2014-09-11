using System;

namespace Hubble_Pong
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (HubblePong game = new HubblePong())
            {
                game.Run();
            }
        }
    }
#endif
}

