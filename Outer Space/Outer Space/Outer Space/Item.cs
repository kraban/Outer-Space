using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Outer_Space
{
    public enum ItemType { nothing, weapon, shield, hull, misc }

    public class Item : GameObject
    {
        public string Description { get; set; }

        public ItemType Type { get; set; }
        public bool RecentlyAcquired;
        private float recentlyAcquiredOpacity;
        private bool recentlyAcquiredflip;
        public Use UseItem { get; set; }
        public int NumberOfItems { get; set; }

        public Item(Use useItem, ItemType type, Texture2D texture, string description)
            : base()
        {
            if (type != ItemType.misc)
            {
                this.Colour = new Color(Globals.Randomizer.Next(50, 255), Globals.Randomizer.Next(50, 255), Globals.Randomizer.Next(50, 255));
            }
            this.Depth = 0.1f;
            this.UseItem = useItem;
            this.Type = type;
            this.Texture = texture;
            Description = description;
            this.NumberOfItems = 1;
        }

        public delegate void Use(Player player, Item item);

        public static void Nothing(Player player, Item item)
        { }

        public static void HealPlayer(Player player, Item item)
        {
            if (player.Health.Value < player.Health.MaxValue)
            {
                player.Health.Change(player.Health.MaxValue / 10);
                if (item.NumberOfItems > 1)
                {
                    item.NumberOfItems--;
                }
                else
                {
                    item.Dead = true;
                }
            }
        }

        public bool PressedRight()
        {
            if (HoverOver() && Globals.MState.RightButton == ButtonState.Pressed && Globals.PrevMState.RightButton == ButtonState.Released)
            {
                return true;
            }
            return false;
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
        }

        public void DrawInventory(SpriteBatch spriteBatch, Vector2 position)
        {
            Position = position;
            Draw(spriteBatch);

            if (NumberOfItems > 1)
            {
                spriteBatch.DrawString(TextureManager.SpriteFont15, NumberOfItems.ToString(), new Vector2(Position.X + 15, Position.Y + 8), Color.White);
            }

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
