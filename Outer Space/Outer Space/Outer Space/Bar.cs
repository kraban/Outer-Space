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
    public class Bar : GameObject
    {
        // Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Value { get; private set; }
        public Color BarColor { get; private set; }

        // Constructor(s)
        public Bar(Vector2 position, int width, int height, float value, Color color)
            : base()
        {
            this.Texture = TextureManager.pixel;
            this.Position = position;
            this.Width = width;
            this.Height = height;
            this.Value = value;
            this.BarColor = color;
        }

        public float Change(float value)
        {
            Value += value;
            float overflow = 0;
            if (Value > Width)
            {
                overflow = Value - Width;
                Value = Width;
            }
            else if (Value < 0)
            {
                overflow = Value;
                Value = 0;
            }
            return overflow;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Value, Height), null, BarColor, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
            if (BarColor.A == 255)
            {
                spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.1f); 
            }
        }
    }
}
