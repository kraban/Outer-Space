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
        public float MaxValue { get; set; }
        public Color BarColor { get; private set; }
        private int[,] bar;

        // Constructor(s)
        public Bar(Vector2 position, int width, int height, float value, Color color)
            : base()
        {
            this.Texture = TextureManager.pixel;
            this.Position = position;
            this.Width = width;
            this.Height = height;
            this.Value = value;
            this.MaxValue = value;
            this.BarColor = color;
            this.bar = new int[width, height];
            this.Depth = 0.5f;
        }

        public float Change(float value)
        {
            Value += value;
            float overflow = 0;
            if (Value > MaxValue)
            {
                overflow = Value - MaxValue;
                Value = MaxValue;
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
            // Text
            if (Globals.MRectangle.Intersects(new Rectangle((int)Position.X, (int)Position.Y, Width * 2, Height * 2)))
            {
                float value = (float)Math.Round(Value + 0.5);
                spriteBatch.DrawString(TextureManager.SpriteFont15, value + " /" + MaxValue.ToString("0"), new Vector2(Position.X + Width + 10 - TextureManager.SpriteFont15.MeasureString(Value.ToString("0") + " /" + MaxValue.ToString("0")).X / 2, Position.Y + Height / 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            }

            // Draw bar
            for (int i = 0; i < bar.GetLength(0) * (Value / MaxValue); i++)
            {
                for (int j = 0; j < bar.GetLength(1); j++)
                {
                    spriteBatch.Draw(TextureManager.pixel, new Rectangle((int)Position.X + i * 2 + j * 2, (int)Position.Y + j * 2, 2, 2), null, BarColor * (j <= Height / 2 ? ((float)j / ((float)Height / 2f)) : (2f - ((float)j / ((float)Height / 2f)))), 0f, Vector2.Zero, SpriteEffects.None, Depth);
                }
            }

            // Border
            for (int i = 0; i < bar.GetLength(0); i++)
            {
                spriteBatch.Draw(TextureManager.pixel, new Rectangle((int)Position.X + i * 2 + 2, (int)Position.Y + 2, 2, 2), Color.DarkGray);
                spriteBatch.Draw(TextureManager.pixel, new Rectangle((int)Position.X + i * 2 + bar.GetLength(1) * 2 - 2, (int)Position.Y + (bar.GetLength(1) - 1) * 2, 2, 2), Color.DarkGray);
            }
            for (int i = 0; i < bar.GetLength(1) - 1; i++)
            {
                spriteBatch.Draw(TextureManager.pixel, new Rectangle((int)Position.X + i * 2 + 2, (int)Position.Y + i * 2 + 2, 2, 2), Color.DarkGray);
                spriteBatch.Draw(TextureManager.pixel, new Rectangle((int)Position.X + i * 2 + 2 + bar.GetLength(0) * 2, (int)Position.Y + i * 2 + 2, 2, 2), Color.DarkGray);
            }
        }
    }
}
