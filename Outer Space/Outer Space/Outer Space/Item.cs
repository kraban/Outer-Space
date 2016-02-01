using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Outer_Space
{
    public enum ItemType { nothing, weapon, shield, hull }

    public class Item : GameObject
    {
        public string Description { get; set; }

        public ItemType Type { get; set; }
        public bool RecentlyAcquired;
        private float recentlyAcquiredOpacity;
        private bool recentlyAcquiredflip;
        internal Texture2D textureBackground;

        public Item()
            : base()
        {
            Description = "";
            this.Colour = new Color(Globals.Randomizer.Next(50, 255), Globals.Randomizer.Next(50, 255), Globals.Randomizer.Next(50, 255));
            this.Depth = 0.1f;

            this.Type = ItemType.nothing;
            this.textureBackground = TextureManager.none;
        }

        public bool HoverOver()
        {
            if (Globals.MRectangle.Intersects(Box))
            {
                RecentlyAcquired = false;
                return true;
            }
            return false;
        }

        public bool Pressed()
        {
            if (HoverOver() && Globals.MState.LeftButton == ButtonState.Pressed && Globals.PrevMState.LeftButton == ButtonState.Released)
            {
                return true;
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(textureBackground, Position, null, Color.White, Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), Size, SpriteEffects.None, Depth + 0.1f);
        }

        public void DrawInventory(SpriteBatch spriteBatch, Vector2 position)
        {
            Position = position;
            Draw(spriteBatch);

            if (RecentlyAcquired)
            {
                if (recentlyAcquiredOpacity > 0.95f || recentlyAcquiredOpacity < 0.05f)
                {
                    recentlyAcquiredflip = !recentlyAcquiredflip;
                }
                if (recentlyAcquiredflip == false)
                {
                    recentlyAcquiredOpacity = MathHelper.Lerp(recentlyAcquiredOpacity, 1, 0.05f);
                }
                else
                {
                    recentlyAcquiredOpacity = MathHelper.Lerp(recentlyAcquiredOpacity, 0, 0.05f);
                }
                spriteBatch.DrawString(TextureManager.SpriteFont15, "NEW", new Vector2(Position.X - Texture.Width / 2, Position.Y - Texture.Height / 2), Color.White * recentlyAcquiredOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            }

            if (HoverOver())
            {
                Text.TextDifferentColor(spriteBatch, Description, new Vector2(0, 0), 1f, TextureManager.SpriteFont15, false);
            }
        }

    }
}
