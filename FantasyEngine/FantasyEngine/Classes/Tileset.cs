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

        public Rectangle TileSize { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public int NbTileX { get { return (TileSize == Rectangle.Empty ? texture.Width : TileSize.Width) / TileWidth; } }

        public Tileset(Texture2D texture, Rectangle tileSize, int tileWidth, int tileHeight)
        {
            this.texture = texture;
            TileSize = tileSize;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }

        public Tileset(Texture2D texture, int tileWidth, int tileHeight)
            : this(texture, Rectangle.Empty, tileWidth, tileHeight)
        {
        }

        public Rectangle? GetSourceRectangle(uint NoTile)
        {
            if (TileWidth == 0 || TileHeight == 0 || texture == null)
                return null;

            int tileX = (int)NoTile % NbTileX;
            int tileY = (int)NoTile / NbTileX;
            return new Rectangle(tileX * TileWidth + TileSize.X, tileY * TileHeight + TileSize.Y, TileWidth, TileHeight);
        }
    }
}
