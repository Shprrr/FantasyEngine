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
        /// Adds a outline rectangle to a batch of sprites for rendering using the specified texture,
        /// outline rectangle, and color.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="outlineRectangle">A rectangle that specifies (in screen coordinates) the destination for drawing the outline rectangle.</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="borderWidth">The width of the outline.</param>
        public void DrawOutlineRectangle(Texture2D texture, Rectangle outlineRectangle, Color color, int borderWidth = 1)
        {
            Draw(texture, new Rectangle(outlineRectangle.Left, outlineRectangle.Top, borderWidth, outlineRectangle.Height), color);
            Draw(texture, new Rectangle(outlineRectangle.Right, outlineRectangle.Top, borderWidth, outlineRectangle.Height), color);
            Draw(texture, new Rectangle(outlineRectangle.Left, outlineRectangle.Top, outlineRectangle.Width, borderWidth), color);
            Draw(texture, new Rectangle(outlineRectangle.Left, outlineRectangle.Bottom, outlineRectangle.Width, borderWidth), color);
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
