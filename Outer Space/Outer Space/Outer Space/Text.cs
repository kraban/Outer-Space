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
        private bool shaking;

        // Constructor(s)
        public Text(Vector2 position, string write, Color textColor, int duration, bool shaking)
            : base()
        {
            this.Position = position;
            this.Write = write;
            this.TextColor = textColor;
            this.duration = duration;
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(TextureManager.SpriteFont20, Write, Position, TextColor, Direction, new Vector2(TextureManager.SpriteFont20.MeasureString(Write).X / 2, TextureManager.SpriteFont20.MeasureString(Write).Y / 2), Size, SpriteEffects.None, 0f);
        }

        public override void Update()
        {
            base.Update();

            duration--;
            if (duration < 0)
            {
                Dead = true;
            }

            // Shake
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
