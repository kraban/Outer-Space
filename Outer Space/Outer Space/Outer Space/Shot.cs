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
    class Shot : GameObject
    {
        // Public properties
        public float Damage { get; set; }
        public float ShieldPiercing { get; set; }
        public int Chance { get; set; }
        public List<String> Targets { get; set; }
        public Hit HitTarget { get; set; }

        // Constructor(s)
        public Shot(Vector2 position, float direction, float damage, Hit hit, List<String> targets, float shieldPiercing, int chance)
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
        }

        // Method(s)

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move
            Position += new Vector2((float)Math.Cos(Direction) * 5, (float)Math.Sin(Direction) * 5);

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
                    ship.TakeDamage(shot.Damage * 2, shot.ShieldPiercing, DamageType.laser, false);
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
                ship.SetDamageOverTime(shot.Damage / 6, 6, shot.ShieldPiercing);
                shot.Dead = true;
            }
        }

        public static void UpdateChanceToExplode(Level level, Shot shot)
        {
            if (Globals.Randomizer.Next(0, 101) < 1)
            {
                for (int i = 0; i < 6; i++)
			    {
                    level.ToAdd.Add(new Shot(shot.Position, (float)(Math.PI * 2 / 6f) * i, shot.Damage, HitBasic, shot.Targets, shot.ShieldPiercing, shot.Chance));
			    }
                shot.Dead = true;
            }
            HitBasic(level, shot);
        }
    }
}
