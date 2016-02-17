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
        public int Disabled { get; set; }
        public Shoot Method { get; set; }

        // Shot delay
        public List<Shot> ShotsToShoot { get; set; }
        private int shotsToShootTimer;

        // Private variables
        private bool drawDescription;
        private int drawDescriptionTimer;
        private int method;

        // Constructor(s)
        public Weapon(Ship ship, int method, int itemLevel)
            : base(Item.Nothing, ItemType.weapon, TextureManager.weapons[1], "", "Weapon")
        {
            this.Damage = Globals.Randomizer.Next(10 + itemLevel * 5, 20 + itemLevel * 5);
            this.ShieldPiercing = (float)Math.Round(MathHelper.Lerp(0, 0.2f, (float)Globals.Randomizer.NextDouble()), 2);
            this.Chance = Globals.Randomizer.Next(20 + itemLevel * 2, 30 + itemLevel * 2);
            this.Depth = 0.5f;
            this.ShotsToShoot = new List<Shot>();
            this.method = method;
            this.ItemLevel = itemLevel;

            this.TextureBackground = TextureManager.weapons[0];

            Method = ListOfMethods()[method];

            // Initialize weapon
            Method(ship, this, 0, null, true);

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
            methods.Add(FireEveryOther);
            methods.Add(FireScattered);
            methods.Add(FireIncreasingChanceOfTwo);
            methods.Add(FireRandom);
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
            Descriptions.Add("Fire a shot that has a |255,70,0|" + (100 - Chance) + "|W|% chance to break and damage yourself.");
            Descriptions.Add("Only fires every other tile match.");
            Descriptions.Add("Fires 3 to 5 shots in scattered directions.");
            Descriptions.Add("For every shot fired, increase the chance of shooting an extra shot by 15 %.\nWhen an extra shot is fired, the chance is reset back to 0.");
            Descriptions.Add("Fire a shot that warps in time.");

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
                    SoundManager.fire.Play();
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
                drawDescription = false;
            }

            // Disabled
            if (Disabled > 0)
            {
                spriteBatch.Draw(TextureManager.jammed, new Vector2(Position.X - Texture.Width / 2, Position.Y - Texture.Height / 2), Color.White);
            }
        }

        public int ShotDamage(int tilesMatched, Ship shooter)
        {
            if (tilesMatched < 3)
            {
                tilesMatched = 3;
            }
            return Damage + (tilesMatched - 3) * (Damage / 3) + shooter.BonusDamageOneFight + shooter.BonusDamage;
        }

        public delegate void Shoot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize);


        public static void FireStandard(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance)); 
            }
        }

        public static void FireAiming(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                if (level.GameObjects.Any(item => shooter.Targets.Any(target => target == item.GetType().Name)))
                {
                    level.ToAdd.Add(new Shot(shooter.Position, (float)(Math.Atan2((level.GameObjects.First(item => shooter.Targets.Any(target => target == item.GetType().Name)).Position - shooter.Position).Y, (level.GameObjects.First(item => shooter.Targets.Any(target => target == item.GetType().Name)).Position - shooter.Position).X)), weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                }
                else
                {
                    FireStandard(shooter, weapon, tilesMatched, level, false);
                }
            }
        }

        public static void FireCrit(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitCrit, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireDelayEnemyShot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitEnemyShotDelay, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireDamageOverTime(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitDamageOverTime, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireChanceToMiss(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                float miss = 0;
                if (Globals.Randomizer.Next(0, 101) > weapon.Chance)
                {
                    miss = MathHelper.Lerp(-0.5f + shooter.ShipHull.WeaponAccuracy, 0.5f - shooter.ShipHull.WeaponAccuracy, (float)Globals.Randomizer.NextDouble());
                }
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction + miss, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
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
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                for (int i = 0; i < 2; i++)
                {
                    weapon.ShotsToShoot.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                }
                SoundManager.fire.Play();
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
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                if (tilesMatched > 3)
                {
                    weapon.ShotsToShoot.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                }
            }
        }

        public static void FireTwoInV(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction + 0.2f, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction - 0.2f, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireExplosiveShot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.UpdateChanceToExplode, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireMovingShot(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
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
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.UpdateBoomerang, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
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
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.UpdateExtraDamageCollideShot, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
        }

        public static void FireBonusDamageLowerHealth(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter) * (2 - shooter.Health.Value / shooter.Health.MaxValue), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
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
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
            else
            {
                weapon.Chance += Globals.Randomizer.Next(30, 50);
                weapon.Damage += Globals.Randomizer.Next(5, 10);
            }
        }

        public static void FireEveryOther(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                if (weapon.Chance == 100)
                {
                    weapon.Chance = 0;
                    level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                }
                else
                {
                    shooter.KnockBack = 0;
                    weapon.Chance = 100;
                }
            }
            else
            {
                weapon.Chance = 0;
                weapon.Damage *= 3;
            }
        }

        public static void FireScattered(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                for (int i = 0; i < Globals.Randomizer.Next(3, 6); i++)
                {
                    weapon.ShotsToShoot.Add(new Shot(shooter.Position, shooter.Direction + MathHelper.Lerp(-0.5f + shooter.ShipHull.WeaponAccuracy, 0.5f - shooter.ShipHull.WeaponAccuracy, (float)Globals.Randomizer.NextDouble()), weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                }
            }
            else
            {
                weapon.Damage /= 3;
            }
        }

        public static void FireIncreasingChanceOfTwo(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                if (Globals.Randomizer.Next(0, 101) < weapon.Chance)
                {
                    weapon.ShotsToShoot.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                    weapon.Chance = 0;
                }
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.HitBasic, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
                weapon.Chance += 15;
            }
        }

        public static void FireRandom(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            if (!initialize)
            {
                level.ToAdd.Add(new Shot(shooter.Position, shooter.Direction, weapon.ShotDamage(tilesMatched, shooter), Shot.UpdateRandom, shooter.Targets, weapon.ShieldPiercing, weapon.Chance));
            }
            else
            {
                weapon.Damage = (int)(weapon.Damage * 1.5f);
            }
        }

        public static void FireBossV(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            Boss boss = (Boss)shooter;
            float directionRight = (float)Math.Atan2(level.Player.Position.Y - boss.RightShootPosition.Y, level.Player.Position.X - boss.RightShootPosition.X);
            float directionLeft = (float)Math.Atan2(level.Player.Position.Y - boss.LeftShootPosition.Y, level.Player.Position.X - boss.LeftShootPosition.X);
            if (!initialize)
            {
                for (int i = 0; i < 10; i++)
                {
                    weapon.ShotsToShoot.Add(new Shot(boss.RightShootPosition, directionRight, 5, Shot.HitBasic, shooter.Targets, 0, 0));
                    weapon.ShotsToShoot.Add(new Shot(boss.LeftShootPosition, directionLeft, 5, Shot.HitBasic, shooter.Targets, 0, 0));
                }
            }
        }

        public static void FireBossX(Ship shooter, Weapon weapon, int tilesMatched, Level level, bool initialize)
        {
            Boss boss = (Boss)shooter;
            float directionRight = (float)Math.Atan2(level.Player.Position.Y - boss.RightShootPosition.Y, (int)boss.ShipLocation * 100 + 100 - boss.RightShootPosition.X);
            float directionLeft = (float)Math.Atan2(level.Player.Position.Y - boss.LeftShootPosition.Y, (int)boss.ShipLocation * 100 + 300 - boss.LeftShootPosition.X);
            if (!initialize)
            {
                for (int i = 0; i < 10; i++)
                {
                    weapon.ShotsToShoot.Add(new Shot(boss.RightShootPosition, directionRight, 5, Shot.HitBasic, shooter.Targets, 0, 0));
                    weapon.ShotsToShoot.Add(new Shot(boss.LeftShootPosition, directionLeft, 5, Shot.HitBasic, shooter.Targets, 0, 0));
                }
            }
        }
    }
}
