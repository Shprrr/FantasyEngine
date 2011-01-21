using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FantasyEngine.Classes
{
    public abstract class Scene : DrawableGameComponent
    {
        private static Stack<Scene> CurrentsScenes = new Stack<Scene>();

        protected SpriteBatch spriteBatch;

        public Scene(Game game)
            : base(game)
        {
            Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = GameMain.spriteBatch;
        }

        /// <summary>
        /// Return the scene currently active.
        /// </summary>
        /// <returns></returns>
        public static Scene CurrentScene { get { return CurrentsScenes.Peek(); } }

        /// <summary>
        /// Change the main scene currently active.  Clear all sub scene with it.
        /// </summary>
        /// <param name="MainScene"></param>
        public static void ChangeMainScene(Scene MainScene)
        {
            Scene[] oldScenes = new Scene[CurrentsScenes.Count];
            CurrentsScenes.CopyTo(oldScenes, 0);

            CurrentsScenes.Clear();
            MainScene.LoadContent();
            MainScene.Game.Components.Add(MainScene);
            CurrentsScenes.Push(MainScene);

            foreach (Scene oldScene in oldScenes)
            {
                oldScene.Game.Components.Remove(oldScene);
                oldScene.UnloadContent();
            }
        }

        /// <summary>
        /// Add a sub scene to the actual main scene.
        /// </summary>
        /// <param name="SubScene"></param>
        public static void AddSubScene(Scene SubScene)
        {
            SubScene.LoadContent();
            // Don't update anymore the scene below.
            CurrentScene.Enabled = false;
            SubScene.DrawOrder = CurrentScene.DrawOrder + 1;
            SubScene.Game.Components.Add(SubScene);
            CurrentsScenes.Push(SubScene);
        }

        /// <summary>
        /// Remove the top-most sub scene of the actual main scene.
        /// </summary>
        public static void RemoveSubScene()
        {
            if (CurrentsScenes.Count <= 1)
                return;

            Scene oldScene = CurrentsScenes.Pop();
            oldScene.Game.Components.Remove(oldScene);
            oldScene.UnloadContent();
            // The scene below become the current and is now updated.
            CurrentScene.Enabled = true;
        }
    }
}
