﻿using System;
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
    public enum Difficulty { Easy, Medium, Hard }
    class Enemy : Ship
    {
        // Public properties
        public int ShootTimer { get; set; }

        // Constructor(s)
        public Enemy()
            : base((float)Math.PI * 0.5f)
        {
            this.Position = new Vector2((int)ShipLocation * 100 + 200, -100);
            this.Texture = TextureManager.enemyShip;
            this.engineAnimation = TextureManager.enemyShipEngineAnimation;
            this.maxFrame = 1;

            this.Health = new Bar(new Vector2(70, 10), 100, 20, 100, Color.Red);
            this.ShipShield = new Shield(new Vector2(270, 10), 100, 20, 10, Globals.Randomizer.Next(0, Shield.ListOfShieldMethods().Count()));

            // Weapontargets
            foreach (Weapon w in Weapons)
            {
                w.Targets.Add("Player");
            }

            // enemy hull
            ShipHull = new Hull(this, 0);
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Weapons
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].Position = new Vector2(Weapons[i].Texture.Width / 2 + 64, 200 + i * Weapons[i].Texture.Height);
                if (i == SelectedWeapon)
                {
                    spriteBatch.Draw(TextureManager.selected, new Vector2(Weapons[i].Position.X - Weapons[i].Texture.Width / 2, Weapons[i].Position.Y - Weapons[i].Texture.Height / 2), Color.White);
                }
                Weapons[i].Draw(spriteBatch);
            }

            // Hull
            ShipHull.Position = new Vector2(ShipHull.Texture.Width / 2, 200);

            // Shield
            ShipShield.Position = new Vector2(ShipShield.Texture.Width / 2, 264);
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move to normal position after knockback
            Position = new Vector2(Position.X, (float)MathHelper.Lerp(Position.Y, Texture.Height + 40, 0.1f));

            if (level.Started)
            {
                // Shoot
                ShootTimer--;
                if (ShootTimer < 0 && ShipLocation == level.Player.ShipLocation && Weapons[SelectedWeapon].Disabled < 0)
                {
                    ShootTimer = 180;
                    Weapons[SelectedWeapon].Method(this, Weapons[SelectedWeapon], 0, level, false);
                    KnockBack = -3;
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
}
