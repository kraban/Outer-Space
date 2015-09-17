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
    class Enemy : Ship
    {
        // Public properties
        public int ShootTimer { get; set; }

        // Constructor(s)
        public Enemy()
            : base((float)Math.PI * 0.5f)
        {
            this.Position = new Vector2((int)ShipLocation * 100 + 100, 100);
            this.Texture = TextureManager.player;

            this.Health = new Bar(new Vector2(0, 10), 100, 10, 100, Color.Red);
            this.ShipShield = new Shield(new Vector2(0, 0), 100, 10, 100);

            // Weapontargets
            foreach (Weapon w in Weapons)
            {
                w.Targets.Add("Player");
            }
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Shoot
            ShootTimer--;
            if (ShootTimer < 0 && ShipLocation == level.Player.ShipLocation && Weapons[SelectedWeapon].Disabled < 0)
            {
                ShootTimer = 180;
                Weapons[SelectedWeapon].Action(Position, Direction, 0, level);
            }

            // Move
            if (Globals.Randomizer.Next(0, 1001) < 2)
            {
                if (ShipLocation < level.Player.ShipLocation)
                {
                    ShipLocation++;
                    DirectionSpeed = -0.0005f;
                }
                else if (ShipLocation > level.Player.ShipLocation)
                {
                    ShipLocation--;
                    DirectionSpeed = 0.0005f;
                }
            }

            // Die
            if (Health.Value <= 0)
            {
                Dead = true;

                // Pieces
                level.CreatePieces(Position, Texture);
            }
        }
    }
}
