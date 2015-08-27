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
        public bool Moving { get; set; }


        // Constructor(s)
        public Tile(Point tilePosition, TileType type)
            : base()
        {
            this.TilePosition = tilePosition;
            this.Position = new Vector2(TilePosition.X * 64 + (Globals.ScreenSize.X - 64 * 8), TilePosition.Y * 64 + 100);
            this.Type = type;
            this.Texture = TextureManager.tiles[(int)type];
            this.Depth = 1f;
        }

        // Method(s)
        public override void Update()
        {
            base.Update();

            // Move
            if (Position != new Vector2(TilePosition.X * 64 + (Globals.ScreenSize.X - 64 * 8), TilePosition.Y * 64 + 100))
            {
                Vector2 move = new Vector2(TilePosition.X * 64 + (Globals.ScreenSize.X - 64 * 8), TilePosition.Y * 64 + 100) - Position;
                Position += move * 0.06f;

                // set move to stop matching when moving
                if (move.Length() > 5)
                {
                    Moving = true;
                }
                else
                {
                    Moving = false;
                }
            }

            // Hide
            if (Hidden && Size > 0)
            {
                Size = 0;
            }
        }

        public void Hide()
        {
            Hidden = true;
        }

        public void UnHide()
        {
            Hidden = false;
            Size = 1f;
            int type = Globals.Randomizer.Next(0, Enum.GetNames(typeof(TileType)).Length);
            Type = (TileType)type;
            Texture = TextureManager.tiles[type];
            Position = new Vector2(Position.X, -100);
        }
    }
}
