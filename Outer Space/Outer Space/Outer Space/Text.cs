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
using System.Text.RegularExpressions;

namespace Outer_Space
{
    public class Text : GameObject
    {
        // Public properties
        public string Write { get; set; }
        public Color TextColor { get; set; }
        public float MoveDirection { get; set; }
        public float Speed { get; set; }

        // Private variable(s)
        private int duration;
        private int maxDuration;
        private float maxSize;
        private SpriteFont spriteFont;
        private bool flash;

        // Constructor(s)
        public Text(Vector2 position, string write, Color textColor, int duration, float size, bool flash, SpriteFont spriteFont)
            : base()
        {
            this.Position = position;
            this.Write = write;
            this.TextColor = textColor;
            this.duration = duration;
            this.maxDuration = duration;
            this.Size = size;
            this.maxSize = size;
            this.spriteFont = spriteFont;
            if (flash)
            {
                this.flash = flash;
                this.Flash = duration;
            }
        }

        // Method(s)

        // different colors in same string
        public static void TextDifferentColor(SpriteBatch spriteBatch, string s, Vector2 position, float size, SpriteFont spriteFont, bool oneLine)
        {
            string[] strings = s.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            string[] originalStrings = s.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            float offsetX = 0;
            float length = 0;

            if (oneLine)
            {
                for (int i = 1; i < strings.Length; i += 2)
                {
                    length += spriteFont.MeasureString(strings[i]).X;
                } 
            }

            for (int i = 0; i < strings.Length; i += 2)
            {
                for (int j = 1; j < i + 1; j += 2)
                {
                    for (int k = 0; k < Regex.Matches(originalStrings[j], "\n").Count; k++)
                    {
                        strings[i + 1] = strings[i + 1].Insert(0, "\n");
                    }
                }

                if (!strings[i].Contains("W"))
                {
                    spriteBatch.DrawString(spriteFont, strings[i + 1], new Vector2(position.X + offsetX, position.Y), new Color(int.Parse(strings[i].Split(',')[0]), int.Parse(strings[i].Split(',')[1]), int.Parse(strings[i].Split(',')[2])), 0f, new Vector2(length / 2, 0), size, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.DrawString(spriteFont, strings[i + 1], new Vector2(position.X + offsetX, position.Y), Color.White, 0f, new Vector2(length / 2, 0), size, SpriteEffects.None, 0f);
                }
                offsetX += spriteFont.MeasureString(strings[i + 1]).X * size;
                if (i + 2 < strings.Length && strings[i + 3].Contains('\n'))
                {
                    offsetX = 0;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Write.Contains("|"))
            {
                spriteBatch.DrawString(spriteFont, Write, Position, TextColor * Opacity, Direction, new Vector2(TextureManager.SpriteFont20.MeasureString(Write).X / 2, TextureManager.SpriteFont20.MeasureString(Write).Y / 2), Size, SpriteEffects.None, Depth); 
            }
            else // different colors in same string
            {
                TextDifferentColor(spriteBatch, Write, Position, Size, spriteFont, true);
            }
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            Update();
        }

        public override void Update()
        {
            base.Update();

            Position += new Vector2((float)Math.Cos(MoveDirection) * Speed, (float)Math.Sin(MoveDirection) * Speed);

            duration--;
            if (!flash)
            {
                Size -= maxSize / maxDuration;
                Opacity = Size;
            }
            if (duration < 0)
            {
                Dead = true;
            }
        }
    }
}
