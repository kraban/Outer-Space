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
    public abstract class GameObject
    {
        // Public properties
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Box { get { return new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height); } }
        public float Size { get; set; }
        public float Direction { get; set; }
        public float Depth { get; set; }
        public bool Dead { get; set; }
        public Color Colour { get; set; }
        public Texture2D TextureBackground { get; set; }
        public float Opacity { get; set; }

        // Flash
        public int Flash { get; set; }
        private bool flip;

        // Constructor(s)
        public GameObject()
        {
            // Default texture
            this.Texture = TextureManager.none;
            this.TextureBackground = TextureManager.none;
            this.Size = 1;
            this.Colour = Color.White;
            this.Opacity = 1;
            this.Depth += 0.1f;
        }

        // Method(s)
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Colour * Opacity, Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), Size, SpriteEffects.None, Depth);
            spriteBatch.Draw(TextureBackground, Position, null, Color.White, Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), Size, SpriteEffects.None, Depth + 0.1f);
        }

        public virtual void Update()
        {
            if (Flash > 0)
            {
                Flash--;
                if (Flash == 0)
                {
                    Opacity = 1;
                }
                if (Opacity < 0.05f || Opacity > 0.95f)
                {
                    flip = !flip;
                }
                if (flip)
                {
                    Opacity = MathHelper.Lerp(Opacity, 0, 0.05f);
                }
                else
                {
                    Opacity = MathHelper.Lerp(Opacity, 1, 0.05f);
                }
            }
        }

        public virtual void UpdateLevel(Level level)
        { }

        public bool OutsideScreen()
        {
            if (Position.X + Texture.Width / 2 < 0 || Position.X - Texture.Width / 2 > Globals.ScreenSize.X || Position.Y + Texture.Height / 2 < 0 || Position.Y - Texture.Height / 2 > Globals.ScreenSize.Y)
            {
                return true;
            }
            return false;
        }
    }
}
