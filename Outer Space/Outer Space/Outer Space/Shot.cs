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
        public Hit Effect { get; set; }

        // Constructor(s)
        public Shot(Vector2 position, float direction, float damage, Hit hit, List<String> targets, float shieldPiercing, int chance)
        {
            this.Position = position;
            this.Direction = direction;
            this.Damage = damage;
            this.Effect = hit;
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

            // Hit targets
            if (level.GameObjects.Any(item => Targets.Any(target => target == item.GetType().Name) && item.Box.Intersects(Box)))
            {
                Ship ship = (Ship)level.GameObjects.First(item => Targets.Any(target => target == item.GetType().Name) && item.Box.Intersects(Box));
                Effect(ship, level, this);
                Dead = true;
            }

            // Hit rock
            if (level.GameObjects.Any(item => item.GetType().Name == "Rock" && item.Box.Intersects(Box)))
            {
                Rock rock = (Rock)level.GameObjects.First(item => item.GetType().Name == "Rock" && item.Box.Intersects(Box));
                rock.Dead = true;
                Dead = true;
            }
        }

        public delegate void Hit(Ship ship, Level level, Shot shot);

        public static void HitBasic(Ship ship, Level level, Shot shot)
        {
            ship.TakeDamage(shot.Damage, shot.ShieldPiercing, DamageType.laser);
        }

        public static void HitCrit(Ship ship, Level level, Shot shot)
        {
            if (Globals.Randomizer.Next(0, 101) < shot.Chance)
            {
                ship.TakeDamage(shot.Damage * 2, shot.ShieldPiercing, DamageType.laser);
                level.CombatText("CRIT!");
            }
            else
            {
                ship.TakeDamage(shot.Damage, shot.ShieldPiercing, DamageType.laser);
            }
        }

        public static void HitEnemyShotDelay(Ship ship, Level level, Shot shot)
        {
            ship.TakeDamage(shot.Damage, shot.ShieldPiercing, DamageType.laser);
            if (Globals.Randomizer.Next(0, 101) < shot.Chance)
            {
                ship.Weapons[Globals.Randomizer.Next(0, ship.Weapons.Count)].Disabled = 120;
                level.CombatText(ship.GetType().Name + " Weapon Jammed!");
            }
        }

        public static void HitDamageOverTime(Ship ship, Level level, Shot shot)
        {
            ship.TakeDamage(shot.Damage, shot.ShieldPiercing, DamageType.laser);
            ship.SetDamageOverTime(shot.Damage / 6, 6, shot.ShieldPiercing);
        }
    }
}
