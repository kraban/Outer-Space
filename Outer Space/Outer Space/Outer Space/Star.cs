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
    public class Star : GameObject
    {
        // Public properties
        public float Speed { get; set; }

        // Constructor(s)
        public Star(Vector2 position)
        {
            this.Position = position;
            this.Texture = TextureManager.pixel;
            this.Speed = MathHelper.Lerp(0.3f, 0.7f, (float)Globals.Randomizer.NextDouble());
            this.Depth = 1;
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, 3, 3), null, Color.Yellow, 0f, new Vector2(0, 0), SpriteEffects.None, 1f);
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            Update();
        }

        public override void Update()
        {
            base.Update();

            // Move
            Position += new Vector2(Speed, 0);

            // Outside Screen
            if (Position.X > Globals.ScreenSize.X + 100)
            {
                Dead = true;
            }
        }
    }
}
