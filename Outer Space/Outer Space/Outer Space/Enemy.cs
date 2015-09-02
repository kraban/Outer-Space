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
    class Enemy : GameObject
    {
        // Public properties
        public Bar Health { get; set; }

        // Private variable(s)
        private int shootTimer;

        // Constructor(s)
        public Enemy()
            : base()
        {
            this.Position = new Vector2(100, 100);
            this.Texture = TextureManager.player;
            this.Direction = (float)Math.PI * 0.5f;

            this.Health = new Bar(new Vector2(0, 0), 100, 20, 10, Color.Red);
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Shoot
            shootTimer--;
            if (shootTimer < 0)
            {
                shootTimer = 180;
                level.ToAdd.Add(new Shot(new Vector2(Position.X, Position.Y + 50), Direction, 25));
            }

            // Die
            if (Health.Value <= 0)
            {
                Dead = true;

                // Pieces
                for (int i = 0; i < Globals.Randomizer.Next(10, 15); i++)
                {
                    level.ToAdd.Add(new Piece(new Vector2(Position.X + Globals.Randomizer.Next(-20, 20), Position.Y + Globals.Randomizer.Next(-20, 20)), Texture));
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Health.Draw(spriteBatch);
        }
    }
}
