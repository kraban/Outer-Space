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
        private bool shaking;
        private Color opacityColor;

        // Constructor(s)
        public Text(Vector2 position, string write, Color textColor, int duration, bool shaking, float size)
            : base()
        {
            this.Position = position;
            this.Write = write;
            this.TextColor = textColor;
            this.duration = duration;
            this.maxDuration = duration;
            this.shaking = shaking;
            this.Size = size;
            this.maxSize = size;
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(TextureManager.SpriteFont20, Write, Position, opacityColor, Direction, new Vector2(TextureManager.SpriteFont20.MeasureString(Write).X / 2, TextureManager.SpriteFont20.MeasureString(Write).Y / 2), Size, SpriteEffects.None, Depth);
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

            // Shake
            if (shaking)
            {
                if (duration % 2 == 0)
                {
                    Direction -= 0.2f;
                }
                else
                {
                    Direction += 0.2f;
                }
            } 
        }
    }
}
