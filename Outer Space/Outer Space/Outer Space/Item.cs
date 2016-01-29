using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Outer_Space
{
    public enum ItemType { trash, weapon, shield, hull }

    public class Item : GameObject
    {
        public string Description { get; set; }

        public ItemType Type { get; set; }
        public bool RecentlyAcquired;
        private float recentlyAcquiredOpacity;
        private bool recentlyAcquiredflip;

        public Item()
            : base()
        {
            Description = "";

            this.Type = ItemType.trash;
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
                spriteBatch.DrawString(TextureManager.SpriteFont15, "NEW", Position, Color.White * recentlyAcquiredOpacity);

            }

            if (HoverOver())
            {
                Text.TextDifferentColor(spriteBatch, Description, new Vector2(0, 0), 1f, TextureManager.SpriteFont15, false);
            }
        }

    }
}
