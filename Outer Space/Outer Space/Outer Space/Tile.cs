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
        public Point TilePosition { get; set; }
        public bool Hidden { get; private set; }


        // Constructor(s)
        public Tile(Point tilePosition, TileType type)
            : base()
        {
            this.TilePosition = tilePosition;
            this.Position = new Vector2(TilePosition.X * 64 + 100, TilePosition.Y * 64 + 100);
            this.Type = type;
            this.Texture = TextureManager.tiles[(int)type];
        }

        // Method(s)
        public override void Update()
        {
            base.Update();

            // Move
            if (Position != new Vector2(TilePosition.X * 64 + 100, TilePosition.Y * 64 + 100))
            {
                Vector2 move = new Vector2(TilePosition.X * 64 + 100, TilePosition.Y * 64 + 100) - Position;
                //move.Normalize();
                Position += move * 0.04f;
            }

            // Hide
            if (Hidden && Size > 0)
            {
                Size -= 0.02f;
            }
        }

        public void Hide()
        {
            Hidden = true;
        }
    }
}
