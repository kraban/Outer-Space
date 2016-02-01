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
    public class Weapon : Item
    {
        // Public properties
        public int Damage { get; set; }
        public float ShieldPiercing { get; set; }
        public int Chance { get; set; }
        public int Action { get; set; }
        public List<Shoot> ShootMethods { get; set; }
        public List<string> Descriptions { get; set; }
        public List<string> Targets { get; set; }
        public int Disabled { get; set; }
        public Shoot CurrentMethod { get { return ShootMethods[Action]; } set { ShootMethods[Action] = value; } }

        // Shot delay
        List<Shot> ShotsToShoot { get; set; }
        private int shotsToShootTimer;

        // Private variables
        private bool drawDescription;
        private int drawDescriptionTimer;

        // Constructor(s)
        public Weapon(Ship ship)
            : base()
        {
            this.Type = ItemType.weapon;
            this.Damage = Globals.Randomizer.Next(10, 20);
            this.ShieldPiercing = (float)Math.Round(Globals.Randomizer.NextDouble(), 2);
            this.Chance = Globals.Randomizer.Next(20, 30);
            this.Depth = 0.5f;
            this.ShotsToShoot = new List<Shot>();

            this.textureBackground = TextureManager.weapons[0];
            this.Texture = TextureManager.weapons[1];

            ShootMethods = new List<Shoot>();
            ShootMethods.Add(FireStandard);
            ShootMethods.Add(FireAiming);
            ShootMethods.Add(FireCrit);
            ShootMethods.Add(FireDelayEnemyShot);
            ShootMethods.Add(FireDamageOverTime);
            ShootMethods.Add(FireChanceToMiss);
            ShootMethods.Add(FireThreeShots);
            ShootMethods.Add(FireMatchFourExtraShot);
            ShootMethods.Add(FireTwoInV);
            ShootMethods.Add(FireExplosiveShot);
            ShootMethods.Add(FireMovingShot);
            ShootMethods.Add(FireBoomerang);
            ShootMethods.Add(FireExtraDamageCollideShot);
            ShootMethods.Add(FireBonusDamageLowerHealth);
            ShootMethods.Add(FireChanceToBreak);

            Action = Globals.Randomizer.Next(0, ShootMethods.Count);

            // Initialize weapon
            ShootMethods[Action](ship, 0, null, true);

            Targets = new List<string>();

            // Description
            LoadDescriptions();
        }

        // Method(s)
        public void LoadDescriptions()
        {
            Descriptions = new List<string>();
            Descriptions.Add("Shoot a standard shot");
            Descriptions.Add("Shoot a shot that aims at a random target");
            Descriptions.Add("Shoot a shot that has a |255,70,0|" + Chance + "|W|% chance to deal double damage");
            Descriptions.Add("Shoot a shot that has a |255,70,0|" + Chance + "|W|% chance to disable|W|\na random target weapon for a few seconds");
            Descriptions.Add("Shoot a shot that deals |255,0,0|" + Damage + "|W| damage over a few seconds");
            Descriptions.Add("Shoot a shot that has a |255,70,0|" + (100 - Chance) + "|W|% chance to shoot in a random direction.");
            Descriptions.Add("Shoot a burst with three shots.");
            Descriptions.Add("Shoot a extra shot when four or more weapon tiles is matched.");
            Descriptions.Add("Shoot two shots in a V pattern.");
            Descriptions.Add("Fire a shot that has a small chance to explode when in air.");
            Descriptions.Add("Fire a shot that has a |255,70,0|" + Chance + "|W|% chance to move you.");
            Descriptions.Add("Fire a shot that has a small chance to boomerang back to you.");
            Descriptions.Add("Fire a shot that, when colliding with another shot, will steal 25% damage from it.");
            Descriptions.Add("Fire a shot that deals 1% extra damage for each percent of missing health.");
            Descriptions.Add("Fire a shot that has a |255,70,0|" + (100 - Chance) + "|W| chance to break and damage yourself.");

            Description = "255,255,255|Damage: |255,0,0|" + Damage + "|W|\nShield Piercing: |0,0,255|" + ShieldPiercing * 100 + "|W|%|W|\n" + Descriptions[Action];
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            if (Disabled >= 0)
            {
                Disabled--;
            }

            // Shots to shoot
            if (ShotsToShoot.Count > 0 && Disabled < 0)
            {
                shotsToShootTimer++;
                if (shotsToShootTimer > 5)
	            {
		            shotsToShootTimer = 0;
                    level.ToAdd.Add(ShotsToShoot[0]);
                    ShotsToShoot.RemoveAt(0); 
	            }
            }

            string key = "D" + (level.Player.SelectedWeapon + 1);
            if (Globals.KState.GetPressedKeys().Any(item => item.ToString() == key))
            {
                drawDescriptionTimer++;
            }

            if ((level.Player.Weapons[level.Player.SelectedWeapon] == this &&  drawDescriptionTimer > 40 || Globals.MRectangle.Intersects(Box)))
            {
                drawDescription = true;
            }
            else
            {
                drawDescription = false;
            }
            
            if (!Globals.KState.GetPressedKeys().Any(item => item.ToString() == key) && drawDescriptionTimer > 40)
            {
                drawDescriptionTimer = 0;
                drawDescription = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Description
            if (drawDescription)
            {
                Text.TextDifferentColor(spriteBatch, Description, new Vector2(Position.X + Texture.Width / 2 + 20, Position.Y - Texture.Height / 2), 1f, TextureManager.SpriteFont15, false);
            }

            // Disabled
            if (Disabled > 0)
            {
                spriteBatch.Draw(TextureManager.jammed, new Vector2(Position.X - Texture.Width / 2, Position.Y - Texture.Height / 2), Color.White);
            }
        }

        public int ShotDamage(int tilesMatched)
        {
            if (tilesMatched < 3)
            {
                tilesMatched = 3;
            }
            return Damage + (tilesMatched - 3) * (Damage / 3);
        }

        public delegate void Shoot(Ship shooter, int tilesMatched, Level level, bool initialize);


        public void FireStandard(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance)); 
            }
        }

        public void FireAiming(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, (float)(Math.Atan2((level.GameObjects.First(item => Targets.Any(target => target == item.GetType().Name)).Position - shooter.Position).Y, (level.GameObjects.First(item => Targets.Any(target => target == item.GetType().Name)).Position - shooter.Position).X)), ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
            }
        }

        public void FireCrit(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitCrit, Targets, ShieldPiercing, Chance));
            }
        }

        public void FireDelayEnemyShot(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitEnemyShotDelay, Targets, ShieldPiercing, Chance));
            }
        }

        public void FireDamageOverTime(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitDamageOverTime, Targets, ShieldPiercing, Chance));
            }
        }

        public void FireChanceToMiss(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                float miss = 0;
                if (Globals.Randomizer.Next(0, 101) > Chance)
                {
                    miss = MathHelper.Lerp(-0.5f, 0.5f, (float)Globals.Randomizer.NextDouble());
                }
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction + miss, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
            }
            else
            {
                Damage += 15;
                Chance = 40;
            }
        }

        public void FireThreeShots(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
                for (int i = 0; i < 2; i++)
                {
                    ShotsToShoot.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
                }
            }
            else
            {
                Damage -= 6;
            }
        }

        public void FireMatchFourExtraShot(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
                if (tilesMatched > 3)
                {
                    ShotsToShoot.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
                }
            }
        }

        public void FireTwoInV(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction + 0.2f, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction - 0.2f, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
            }
        }

        public void FireExplosiveShot(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.UpdateChanceToExplode, Targets, ShieldPiercing, Chance));
            }
        }

        public void FireMovingShot(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.HitBasic, Targets, ShieldPiercing, Chance));
                int random = Globals.Randomizer.Next(0, 101);
                if (random < Chance / 2)
                {
                    if ((int)shooter.ShipLocation < 2)
                    {
                        shooter.ShipLocation++;
                    }
                }
                else if (random < Chance)
                {
                    if ((int)shooter.ShipLocation > 0)
                    {
                        shooter.ShipLocation--;
                    }
                }
            }
        }

        public void FireBoomerang(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                List<string> targets = new List<string>();
                for (int i = 0; i < Targets.Count; i++)
                {
                    targets.Add(Targets[i]);
                }
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.UpdateBoomerang, targets, ShieldPiercing, Chance));
            }
            else
            {
                Damage += Globals.Randomizer.Next(5, 10);
            }
        }

        public void FireExtraDamageCollideShot(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched), Shot.UpdateExtraDamageCollideShot, Targets, ShieldPiercing, Chance));
            }
        }

        public void FireBonusDamageLowerHealth(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched) * (2 - shooter.Health.Value / shooter.Health.MaxValue), Shot.HitBasic, Targets, ShieldPiercing, Chance));
            }
        }

        public void FireChanceToBreak(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                if (Globals.Randomizer.Next(0, 101) < 100 - Chance)
                {
                    Disabled = 180;
                    shooter.TakeDamage(Damage, ShieldPiercing, DamageType.laser, false);
                }
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, ShotDamage(tilesMatched) * (2 - shooter.Health.Value / shooter.Health.MaxValue), Shot.HitBasic, Targets, ShieldPiercing, Chance));
            }
            else
            {
                Chance += Globals.Randomizer.Next(30, 50);
                Damage += Globals.Randomizer.Next(5, 10);
            }
        }

        public void FireRam(Ship shooter, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
            }
        }
    }
}
