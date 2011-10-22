using System;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

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
            try
            {
                using (GameMain game = new GameMain())
                {
                    game.Run();
                }
            }
#if !DEBUG
            catch (NotImplementedException)
            {
                MessageBox.Show("Sorry, this feature is not implemented yet.", Assembly.GetEntryAssembly().GetName().Name);
            }
#endif
            catch (NoSuitableGraphicsDeviceException exc)
            {
#if DEBUG
                MessageBox.Show(exc.ToString(), exc.GetType().Name);
#endif

                if (GraphicsAdapter.UseReferenceDevice)
                    MessageBox.Show("No suitable graphics are found." + Environment.NewLine + "You can't play this game.",
                        Assembly.GetEntryAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    if (MessageBox.Show("No suitable graphics are found." + Environment.NewLine + "Do you still want to play this game ?",
                        Assembly.GetEntryAssembly().GetName().Name, MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        GraphicsAdapter.UseReferenceDevice = true;
                        Main(args);
                    }
            }
        }
    }
#endif
}

