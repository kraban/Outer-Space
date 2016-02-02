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
    public enum TileType { shield, right, left, cog, shoot }

    public class Tile : GameObject
    {
        // Public properties
        public TileType Type { get; set; }
        public Point TilePosition { get; set; }
        public bool Hidden { get; private set; }
        public bool Moving { get; set; }
        public int ManuallyMoved { get; set; }
        public bool Mine { get { return mine; } set { mine = value; MineSize = 5; } }
        public float MineSize { get; set; }
        private bool mine;


        // Constructor(s)
        public Tile(Point tilePosition, TileType type)
            : base()
        {
            this.TilePosition = tilePosition;
            this.Position = new Vector2(TilePosition.X * 64 + (Globals.ScreenSize.X - 64 * 8), TilePosition.Y * 64 + 100);
            this.Type = type;
            this.Texture = TextureManager.tiles[(int)type];
            this.Depth = 0.8f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Mine)
            {
                MineSize = MathHelper.Lerp(MineSize, 1, 0.1f);
                spriteBatch.Draw(TextureManager.jammed, Position, null, Color.White, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), MineSize, SpriteEffects.None, Depth - 0.1f);
            }
        }

        // Method(s)
        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);
            base.Update();

            if (ManuallyMoved >= 0)
            {
                ManuallyMoved--;
            }

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

        public void UnHide(Player player)
        {
            Hidden = false;
            Mine = false;
            Size = 1f;
            Type = player.ShipHull.TileChance[Globals.Randomizer.Next(0, player.ShipHull.TileChance.Count)];
            Texture = TextureManager.tiles[(int)Type];
            Position = new Vector2(Position.X, -100);
        }
    }
}
