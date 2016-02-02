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
        public List<string> Descriptions { get; set; }
        public List<string> Targets { get; set; }
        public int Disabled { get; set; }
        public Shoot Method { get; set; }

        // Shot delay
        List<Shot> ShotsToShoot { get; set; }
        private int shotsToShootTimer;

        // Private variables
        private bool drawDescription;
        private int drawDescriptionTimer;
        private int method;

        // Constructor(s)
        public Weapon(Ship ship, int method)
            : base()
        {
            this.Type = ItemType.weapon;
            this.Damage = Globals.Randomizer.Next(10, 20);
            this.ShieldPiercing = (float)Math.Round(Globals.Randomizer.NextDouble(), 2);
            this.Chance = Globals.Randomizer.Next(20, 30);
            this.Depth = 0.5f;
            this.ShotsToShoot = new List<Shot>();
            this.method = method;

            this.TextureBackground = TextureManager.weapons[0];
            this.Texture = TextureManager.weapons[1];

            Method = ListOfMethods()[method];

            // Initialize weapon
            Method(ship, this, 0, null, true);

            Targets = new List<string>();

            // Description
            LoadDescriptions();
        }

        // Method(s)
        public static List<Shoot> ListOfMethods()
        {
            List<Shoot> methods = new List<Shoot>();
            methods.Add(FireStandard);
            methods.Add(FireAiming);
            methods.Add(FireCrit);
            methods.Add(FireDelayEnemyShot);
            methods.Add(FireDamageOverTime);
            methods.Add(FireChanceToMiss);
            methods.Add(FireThreeShots);
            methods.Add(FireMatchFourExtraShot);
            methods.Add(FireTwoInV);
            methods.Add(FireExplosiveShot);
            methods.Add(FireMovingShot);
            methods.Add(FireBoomerang);
            methods.Add(FireExtraDamageCollideShot);
            methods.Add(FireBonusDamageLowerHealth);
            methods.Add(FireChanceToBreak);
            return methods;

        }

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

            Description = "255,255,255|Damage: |255,0,0|" + Damage + "|W|\nShield Piercing: |0,0,255|" + ShieldPiercing * 100 + "|W|%|W|\n" + Descriptions[method];
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

        public delegate void Shoot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize);


        public static void FireStandard(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance)); 
            }
        }

        public static void FireAiming(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, (float)(Math.Atan2((level.GameObjects.First(item => weapon.Targets.Any(target => target == item.GetType().Name)).Position - shooter.Position).Y, (level.GameObjects.First(item => weapon.Targets.Any(target => target == item.GetType().Name)).Position - shooter.Position).X)), weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireCrit(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitCrit, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireDelayEnemyShot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitEnemyShotDelay, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireDamageOverTime(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitDamageOverTime, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireChanceToMiss(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                float miss = 0;
                if (Globals.Randomizer.Next(0, 101) > weapon.Chance)
                {
                    miss = MathHelper.Lerp(-0.5f, 0.5f, (float)Globals.Randomizer.NextDouble());
                }
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction + miss, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
            else
            {
                weapon.Damage += 15;
                weapon.Chance = 40;
            }
        }

        public static void FireThreeShots(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
                for (int i = 0; i < 2; i++)
                {
                    weapon.ShotsToShoot.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
                }
            }
            else
            {
                weapon.Damage -= 6;
            }
        }

        public static void FireMatchFourExtraShot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
                if (tilesMatched > 3)
                {
                    weapon.ShotsToShoot.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
                }
            }
        }

        public static void FireTwoInV(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction + 0.2f, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction - 0.2f, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireExplosiveShot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.UpdateChanceToExplode, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireMovingShot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
                int random = Globals.Randomizer.Next(0, 101);
                if (random < weapon.Chance / 2)
                {
                    if ((int)shooter.ShipLocation < 2)
                    {
                        shooter.ShipLocation++;
                    }
                }
                else if (random < weapon.Chance)
                {
                    if ((int)shooter.ShipLocation > 0)
                    {
                        shooter.ShipLocation--;
                    }
                }
            }
        }

        public static void FireBoomerang(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                List<string> targets = new List<string>();
                for (int i = 0; i < weapon.Targets.Count; i++)
                {
                    targets.Add(weapon.Targets[i]);
                }
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.UpdateBoomerang, targets, weapon.ShieldPiercing, weapon.Chance));
            }
            else
            {
                weapon.Damage += Globals.Randomizer.Next(5, 10);
            }
        }

        public static void FireExtraDamageCollideShot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched), Shot.UpdateExtraDamageCollideShot, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireBonusDamageLowerHealth(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched) * (2 - shooter.Health.Value / shooter.Health.MaxValue), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireChanceToBreak(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                if (Globals.Randomizer.Next(0, 101) < 100 - weapon.Chance)
                {
                    weapon.Disabled = 180;
                    shooter.TakeDamage(weapon.Damage, weapon.ShieldPiercing, DamageType.laser, false);
                }
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched) * (2 - shooter.Health.Value / shooter.Health.MaxValue), Shot.HitBasic, weapon.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
            else
            {
                weapon.Chance += Globals.Randomizer.Next(30, 50);
                weapon.Damage += Globals.Randomizer.Next(5, 10);
            }
        }
    }
}
