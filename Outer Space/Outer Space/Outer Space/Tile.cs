using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Outer_Space
{
    public enum TileType { shield, up, down, cog, shoot }

    class Tile : GameObject
    {
        // Public properties
        public TileType Type { get; set; }

        // Constructor(s)
        public Tile(Vector2 position, TileType type)
            : base()
        {
            this.Position = position;
            this.Type = type;
            this.Texture = TextureManager.tiles[(int)type];
        }
    }
}
