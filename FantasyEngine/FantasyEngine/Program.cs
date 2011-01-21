using System;

namespace FantasyEngine
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameMain game = new GameMain())
            {
#if !DEBUG
                try
                {
#endif
                    game.Run();
#if !DEBUG
                }
                catch (NotImplementedException)
                {
                    System.Windows.Forms.MessageBox.Show("Sorry, this feature is not implemented yet.");
                }
#endif
            }
        }
    }
#endif
}

