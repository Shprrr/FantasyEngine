using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FantasyEngine.Classes
{
    /// <summary>
    /// Encapsulate special input process.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Number of millisecond to wait to move the cursor when holding a direction.
        /// </summary>
        public const double DELAY_CURSOR = 20 * GameMain.MILLISECOND_PER_FRAME;

        private static GameTime oldTime = new GameTime();
        private static bool oldResultInput = true;
        private static double delay = 0;
        private static Keys[] oldKeys = new Keys[0];

        /// <summary>
        /// Keyboard state on the actual frame only.
        /// </summary>
        public static KeyboardState keyStateDown = new KeyboardState();
        private static KeyboardState oldKeyStateHeld = keyStateHeld;
        /// <summary>
        /// Keyboard state actual.
        /// </summary>
        public static KeyboardState keyStateHeld = new KeyboardState();

        /// <summary>
        /// Freeze inputs for the delay specifed.
        /// </summary>
        /// <param name="keys">Keys who freeze the input</param>
        public static void PutDelay(params Keys[] keys)
        {
            PutDelay(DELAY_CURSOR, keys);
        }

        /// <summary>
        /// Freeze inputs for the delay specifed.
        /// </summary>
        /// <param name="delay">Number of millisecond which is frozen</param>
        /// <param name="keys">Keys who freeze the input</param>
        public static void PutDelay(double delay, params Keys[] keys)
        {
            Input.delay = delay;
            oldKeys = keys;
        }

        /// <summary>
        /// Remove the key in the keystate to not process this key anymore in this frame.
        /// </summary>
        /// <param name="key">Key who has been processed</param>
        public static void CatchKeys(Keys key)
        {
            Keys[] keys = keyStateDown.GetPressedKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == key)
                    keys[i] = Keys.None;
            }

            keyStateDown = new KeyboardState(keys);

            keys = keyStateHeld.GetPressedKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == key)
                    keys[i] = Keys.None;
            }

            keyStateHeld = new KeyboardState(keys);
        }

        /// <summary>
        /// Update inputs for the frame and tell if it needs to be processed.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns>If the input is frozen</returns>
        public static bool UpdateInput(GameTime gameTime)
        {
            // If the gameTime is the same it means the Update is already done previously for that frame.
            if (gameTime.TotalGameTime == oldTime.TotalGameTime)
                return oldResultInput;

            oldTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);

            keyStateHeld = Keyboard.GetState(PlayerIndex.One);

            // Check whose keys are down, but not held.
            List<Keys> downKeys = new List<Keys>();
            foreach (Keys key in keyStateHeld.GetPressedKeys())
            {
                if (!oldKeyStateHeld.IsKeyDown(key))
                {
                    downKeys.Add(key);
                }
            }
            keyStateDown = new KeyboardState(downKeys.ToArray());
            oldKeyStateHeld = keyStateHeld;

            // Free delay if the key isn't hold anymore.
            foreach (Keys oldKey in oldKeys)
            {
                if (keyStateHeld.IsKeyUp(oldKey))
                    delay = 0;
            }

            // Decrease one frame the delay remaining.
            if (delay > 0)
            {
                delay -= gameTime.ElapsedGameTime.TotalMilliseconds;
                oldResultInput = false;
                return false;
            }

            oldResultInput = true;
            return true;
        }
    }
}
