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
    class Piece : GameObject
    {
        // Public properties
        public Rectangle DrawArea { get; set; }
        public float TurnSpeed { get; set; }
        public float Turn { get; set; }
        public float Speed { get; set; }

        // Constructor(s)
        public Piece(Vector2 position, Texture2D texture)
            : base()
        {
            this.TurnSpeed = MathHelper.Lerp(-0.02f, 0.02f, (float)Globals.Randomizer.NextDouble());
            this.Speed = Globals.Randomizer.Next(2, 4);
            this.Position = position;
            this.Texture = texture;
            this.Direction = MathHelper.Lerp(0, (float)Math.PI * 2, (float)Globals.Randomizer.NextDouble());
            this.Size = 0.7f;

            // Piece of texture
            int x = Globals.Randomizer.Next(0, texture.Width / 2);
            int y = Globals.Randomizer.Next(0, texture.Height / 2);
            this.DrawArea = new Rectangle(x, y, x + texture.Width / 4, y + texture.Height / 4);
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, DrawArea, Color.White, Direction + Turn, new Vector2(Texture.Width / 2, Texture.Height / 2), Size, SpriteEffects.None, 0f);
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
            Position += new Vector2((float)Math.Cos(Direction) * Speed, (float)Math.Sin(Direction) * Speed);
            Turn += TurnSpeed;

            // Die
            Size -= 0.01f;
            if (Size < 0.03)
            {
                Dead = true;
            }
        }
    }
}
