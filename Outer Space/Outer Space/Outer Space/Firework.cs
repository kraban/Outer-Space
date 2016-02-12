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
    class Firework : GameObject
    {
        private Vector2 prevPosition;
        private Vector2 prevPrevPosition;
        private Vector2 velocity;

        public Firework(Vector2 position)
        {
            this.Position = position;
            this.prevPosition = position;
            this.prevPrevPosition = position;
            this.Texture = TextureManager.pixel;
            this.Direction = MathHelper.Lerp(0, (float)Math.PI * 2, (float)Globals.Randomizer.NextDouble());
            this.Colour = new Color(Globals.Randomizer.Next(0, 255), Globals.Randomizer.Next(0, 255), Globals.Randomizer.Next(0, 255));
            this.velocity = new Vector2((float)Math.Cos(Direction) * 5, (float)Math.Sin(Direction) * 5);
        }

        public override void Update()
        {
            base.Update();

            prevPrevPosition = prevPosition;
            prevPosition = new Vector2(Position.X, Position.Y);
            Position += velocity;

            if (Position.X < -20 || Position.X > Globals.ScreenSize.X + 20 || Position.Y < -20 || Position.Y > Globals.ScreenSize.Y + 20)
            {
                Dead = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, 6, 6), null, Colour, Direction, new Vector2(3, 3), SpriteEffects.None, Depth);
            spriteBatch.Draw(Texture, new Rectangle((int)prevPosition.X, (int)Position.Y, 6, 6), null, Colour * 0.7f, Direction, new Vector2(3, 3), SpriteEffects.None, Depth);
            spriteBatch.Draw(Texture, new Rectangle((int)prevPrevPosition.X, (int)prevPrevPosition.Y, 6, 6), null, Colour * 0.4f, Direction, new Vector2(3, 3), SpriteEffects.None, Depth);
        }
    }
}
