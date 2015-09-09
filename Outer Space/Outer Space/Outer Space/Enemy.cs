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
        public int ShootTimer { get; set; }
        public Location ShipLocation { get; set; }

        // Constructor(s)
        public Enemy()
            : base()
        {
            this.ShipLocation = Location.middle;
            this.Position = new Vector2((int)ShipLocation * 100 + 100, 100);
            this.Texture = TextureManager.player;
            this.Direction = (float)Math.PI * 0.5f;

            this.Health = new Bar(new Vector2(0, 0), 100, 20, 100, Color.Red);
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Shoot
            ShootTimer--;
            if (ShootTimer < 0 && ShipLocation == level.Player.ShipLocation)
            {
                ShootTimer = 180;
                level.ToAdd.Add(new Shot(new Vector2(Position.X, Position.Y + 50), Direction, 25, Shot.HitBasic));
            }

            // Move
            if (Globals.Randomizer.Next(0, 1001) < 2)
            {
                if (ShipLocation < level.Player.ShipLocation)
                {
                    ShipLocation++;
                }
                else if (ShipLocation > level.Player.ShipLocation)
                {
                    ShipLocation--;
                } 
            }
            Vector2 move = new Vector2((int)ShipLocation * 100 + 100, Position.Y) - Position;
            Position += move * 0.05f;

            // Die
            if (Health.Value <= 0)
            {
                Dead = true;

                // Pieces
                level.CreatePieces(Position, Texture);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Health.Draw(spriteBatch);
        }
    }
}
