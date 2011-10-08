using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Entities
{
    public struct BattleSprite
    {
        public string SpriteName;
        [ContentSerializer(Optional = true)]
        public bool IsTiled;
        [ContentSerializer(Optional = true)]
        public uint TileWidth;
        [ContentSerializer(Optional = true)]
        public uint TileHeight;

        public BattleSprite(string spriteName, bool isTiled = false, uint tileWidth = 0, uint tileHeight = 0)
        {
            SpriteName = spriteName;
            IsTiled = isTiled;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }
    }
}
