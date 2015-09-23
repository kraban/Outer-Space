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
    class Text : GameObject
    {
        // Public properties
        public string Write { get; set; }
        public Color TextColor { get; set; }

        // Private variable(s)
        private int duration;
        private int maxDuration;
        private float maxSize;
        private Color opacityColor;

        // Constructor(s)
        public Text(Vector2 position, string write, Color textColor, int duration, float size)
            : base()
        {
            this.Position = position;
            this.Write = write;
            this.TextColor = textColor;
            this.duration = duration;
            this.maxDuration = duration;
            this.Size = size;
            this.maxSize = size;
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Write.Contains("|"))
            {
                spriteBatch.DrawString(TextureManager.SpriteFont20, Write, Position, opacityColor, Direction, new Vector2(TextureManager.SpriteFont20.MeasureString(Write).X / 2, TextureManager.SpriteFont20.MeasureString(Write).Y / 2), Size, SpriteEffects.None, Depth); 
            }
            else // different colors in same string
            {
                string[] strings = Write.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                float offset = 0;
                float length = 0;

                for (int i = 1; i < strings.Length; i += 2)
                {
                    length += TextureManager.SpriteFont20.MeasureString(strings[i]).X;
                }

                for (int i = 0; i < strings.Length; i += 2)
                {
                    spriteBatch.DrawString(TextureManager.SpriteFont20, strings[i + 1], new Vector2(Position.X + offset, Position.Y), new Color(float.Parse(strings[i].Split(',')[0]), float.Parse(strings[i].Split(',')[1]), float.Parse(strings[i].Split(',')[2])), Direction, new Vector2(length / 2, TextureManager.SpriteFont20.MeasureString(Write).Y / 2), Size, SpriteEffects.None, Depth);
                    offset += TextureManager.SpriteFont20.MeasureString(strings[i + 1]).X * Size;
                }
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
            duration--;
            Size -= maxSize / maxDuration;
            opacityColor = TextColor * Size;
            if (duration < 0)
            {
                Dead = true;
            }
        }
    }
}
