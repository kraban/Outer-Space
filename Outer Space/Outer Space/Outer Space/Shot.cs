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
    public class Shot : GameObject
    {
        // Public properties
        public float Damage { get; set; }
        public float ShieldPiercing { get; set; }
        public int Chance { get; set; }
        public List<string> Targets { get; set; }
        public Hit HitTarget { get; set; }
        public float Speed { get; set; }

        private int timer;
        private float changeSpeed;

        // Constructor(s)
        public Shot(Vector2 position, float direction, float damage, Hit hit, List<string> targets, float shieldPiercing, int chance)
        {
            this.Position = position;
            this.Direction = direction;
            this.Damage = damage;
            this.HitTarget = hit;
            this.Texture = TextureManager.shot;
            this.Targets = new List<string>();
            this.Targets = targets;
            this.ShieldPiercing = shieldPiercing;
            this.Chance = chance;
            this.timer = 60;
            this.Speed = 5;
            this.changeSpeed = 5;
            this.Colour = Color.Red;
            SoundManager.shoot.Play();
        }

        // Method(s)

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            timer--;

            // Move
            if (changeSpeed != Speed)
            {
                Speed = MathHelper.Lerp(Speed, changeSpeed, 0.1f);
            }
            Position += new Vector2((float)Math.Cos(Direction) * Speed, (float)Math.Sin(Direction) * Speed);

            // Outside screen
            if (OutsideScreen())
            {
                Dead = true;
            }

            HitTarget(level, this);

            // Hit rock
            if (level.GameObjects.Any(item => item.GetType().Name == "Rock" && item.Box.Intersects(Box)))
            {
                Rock rock = (Rock)level.GameObjects.First(item => item.GetType().Name == "Rock" && item.Box.Intersects(Box));
                level.CreatePieces(rock.Position, rock.Texture);
                rock.Dead = true;
                Dead = true;
            }
        }

        public static Ship CollisionTarget(Level level, Shot shot)
        {
            if (level.GameObjects.Any(item => shot.Targets.Any(target => target == item.GetType().Name) && item.Box.Intersects(shot.Box)))
            {
                return (Ship)level.GameObjects.First(item => shot.Targets.Any(target => target == item.GetType().Name) && item.Box.Intersects(shot.Box));
            }
            return null;
        }

        public delegate void Hit(Level level, Shot shot);

        public static void HitBasic(Level level, Shot shot)
        {
            if (CollisionTarget(level, shot) != null)
            {
                Ship ship = CollisionTarget(level, shot);
                ship.TakeDamage(shot.Damage, shot.ShieldPiercing, DamageType.laser, false);
                shot.Dead = true;
            }
        }

        public static void HitCrit(Level level, Shot shot)
        {
            if (CollisionTarget(level, shot) != null)
            {
                Ship ship = CollisionTarget(level, shot);
                if (Globals.Randomizer.Next(0, 101) < shot.Chance)
                {
                    ship.TakeDamage(shot.Damage * 1.5f, shot.ShieldPiercing, DamageType.laser, false);
                    level.CombatText("CRIT!");
                }
                else
                {
                    ship.TakeDamage(shot.Damage, shot.ShieldPiercing, DamageType.laser, false);
                }
                shot.Dead = true;
            }
        }

        public static void HitEnemyShotDelay(Level level, Shot shot)
        {
            if (CollisionTarget(level, shot) != null)
            {
                Ship ship = CollisionTarget(level, shot);
                ship.TakeDamage(shot.Damage, shot.ShieldPiercing, DamageType.laser, false);
                if (Globals.Randomizer.Next(0, 101) < shot.Chance)
                {
                    ship.Weapons[Globals.Randomizer.Next(0, ship.Weapons.Count)].Disabled = 120;
                    level.CombatText(ship.GetType().Name + " Weapon Jammed!");
                }
                shot.Dead = true;
            }
        }

        public static void HitDamageOverTime(Level level, Shot shot)
        {
            if (CollisionTarget(level, shot) != null)
            {
                Ship ship = CollisionTarget(level, shot);
                ship.TakeDamage(shot.Damage, shot.ShieldPiercing, DamageType.laser, false);
                ship.SetDamageOverTime(shot.Damage / 12, 6, shot.ShieldPiercing);
                shot.Dead = true;
            }
        }

        public static void UpdateChanceToExplode(Level level, Shot shot)
        {
            if (Globals.Randomizer.Next(0, 101) < 1 && shot.timer < 0)
            {
                for (int i = 0; i < 6; i++)
			    {
                    level.ToAdd.Add(new Shot(shot.Position, (float)(Math.PI * 2 / 6f) * i, shot.Damage, HitBasic, shot.Targets, shot.ShieldPiercing, shot.Chance));
			    }
                shot.Dead = true;
            }
            HitBasic(level, shot);
        }

        public static void UpdateBoomerang(Level level, Shot shot)
        {
            if (Globals.Randomizer.Next(0, 101) < 3 && shot.timer < 0)
            {
                shot.changeSpeed = -shot.changeSpeed;
                shot.HitTarget = Shot.HitBasic;
                if (shot.Targets.Any(item => item == "Player"))
                {
                    shot.Targets = new List<string>();
                    shot.Targets.Add("Enemy");
                    shot.Targets.Add("Boss");
                }
                else
                {
                    shot.Targets = new List<string>();
                    shot.Targets.Add("Player");
                }
            }
            HitBasic(level, shot);
        }

        public static void UpdateExtraDamageCollideShot(Level level, Shot shot)
        {
            if (level.GameObjects.Any(item => item != shot && item.GetType().Name == "Shot" && item.Box.Intersects(shot.Box)))
            {
                Shot hitShot =  (Shot)level.GameObjects.First(item => item != shot && item.GetType().Name == "Shot" && item.Box.Intersects(shot.Box));
                shot.Damage += hitShot.Damage * 0.25f;
                shot.Colour = new Color(0, 255, 255);
                hitShot.Damage *= 0.75f;
                hitShot.Colour = hitShot.Colour * 0.75f;
            }
            HitBasic(level, shot);
        }

        public static void UpdateRandom(Level level, Shot shot)
        {
            shot.Position += new Vector2(Globals.Randomizer.Next(-20, 21), 0);
            HitBasic(level, shot);
        }
    }
}
