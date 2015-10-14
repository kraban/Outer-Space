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

            if (HoverOver())
            {
                Text.TextDifferentColor(spriteBatch, Description, new Vector2(0, 0), 1f, TextureManager.SpriteFont15, false);
            }
        }

    }
}
