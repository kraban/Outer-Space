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
    class Bar : GameObject
    {
        // Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Value { get; private set; }
        public Color BarColor { get; private set; }

        // Constructor(s)
        public Bar(Vector2 position, int width, int height, int value, Color color)
            : base()
        {
            this.Texture = TextureManager.pixel;
            this.Position = position;
            this.Width = width;
            this.Height = height;
            this.Value = value;
            this.BarColor = color;
        }

        public void Change(int value)
        {
            Value += value;
            if (Value > Width)
            {
                Value = Width;
            }
            else if (Value < 0)
            {
                Value = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Value, Height), null, BarColor, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
        }
    }
}
