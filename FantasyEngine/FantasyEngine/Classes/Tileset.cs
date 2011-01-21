using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FantasyEngine.Classes
{
    public class Tileset
    {
        public Texture2D texture;

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public Tileset(Texture2D texture, int tileWidth, int tileHeight)
        {
            this.texture = texture;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
        }

        public Rectangle? GetSourceRectangle(uint NoTile)
        {
            if (TileWidth == 0 || TileHeight == 0 || texture == null)
                return null;

            int nbTileX = texture.Width / TileWidth;
            int tileX = (int)NoTile % nbTileX;
            int tileY = (int)NoTile / nbTileX;
            return new Rectangle(tileX * TileWidth, tileY * TileHeight, TileWidth, TileHeight);
        }
    }
}
