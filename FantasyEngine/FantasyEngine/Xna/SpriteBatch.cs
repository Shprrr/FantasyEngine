using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FantasyEngine.Xna
{
    public class SpriteBatch : Microsoft.Xna.Framework.Graphics.SpriteBatch
    {
        protected Rectangle defaultScissor;
        protected RasterizerState rastState = new RasterizerState();
        public Matrix cameraMatrix = Matrix.Identity;
        private Matrix _OldCameraMatrix = Matrix.Identity;
        /// <summary>
        /// The Matrix for the camera when the SpriteBatch began.
        /// </summary>
        public Matrix OldCameraMatrix
        {
            get { return _OldCameraMatrix; }

            protected set
            {
                _OldCameraMatrix = value;
                _CameraOffset = new Vector2(-OldCameraMatrix.Translation.X, -OldCameraMatrix.Translation.Y);
            }
        }

        private Vector2 _CameraOffset;
        /// <summary>
        /// Offset to draw 0,0 in the current camera.
        /// </summary>
        public Vector2 CameraOffset { get { return _CameraOffset; } }

        public SpriteBatch(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            rastState.ScissorTestEnable = true;
            defaultScissor = graphicsDevice.ScissorRectangle;
        }

        public void BaseBegin()
        {
            base.Begin();
        }

        new public void Begin()
        {
            base.Begin(0, null, null, null, rastState, null, cameraMatrix);
            OldCameraMatrix = cameraMatrix;
        }

        /// <summary>
        /// Change the region where the drawing can occur.
        /// </summary>
        /// <param name="rectangle">Region where the drawing can occur</param>
        public void Scissor(Rectangle rectangle)
        {
            End();
            GraphicsDevice.ScissorRectangle = rectangle;
            Begin();
            OldCameraMatrix = cameraMatrix;
        }

        /// <summary>
        /// Return to default the region where the drawing can occur.
        /// </summary>
        public void ScissorReset() { Scissor(defaultScissor); }
    }
}
