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
        public int Damage { get; set; }
        public Hit HitEnemy { get; set; }

        // Constructor(s)
        public Shot(Vector2 position, float direction, int damage, Hit hit)
        {
            this.Position = position;
            this.Direction = direction;
            this.Damage = damage;
            this.HitEnemy = hit;
            this.Texture = TextureManager.shot;
        }

        // Method(s)
        public delegate void Hit(Enemy enemy, Level level, Shot shot);

        public static void HitBasic(Enemy enemy, Level level, Shot shot)
        {
            enemy.Health.Change(-shot.Damage);
        }

        public static void HitCrit(Enemy enemy, Level level, Shot shot)
        {
            if (Globals.Randomizer.Next(0, 101) < 40)
            {
                enemy.Health.Change(-shot.Damage * 2);
                level.ToAdd.Add(new Text(new Vector2(200, 300), "CRIT!", Color.Red, 90, false, 1.2f));
            }
            else
            {
                enemy.Health.Change(-shot.Damage);
            }
        }

        public static void HitEnemyShotDelay(Enemy enemy, Level level, Shot shot)
        {
            enemy.Health.Change(-shot.Damage);
            if (Globals.Randomizer.Next(0, 101) < 30)
            {
                enemy.ShootTimer = 300;
                level.ToAdd.Add(new Text(new Vector2(200, 300), "Enemy Weapon Jammed!", Color.Red, 90, false, 1.2f));
            }
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move
            Position += new Vector2((float)Math.Cos(Direction) * 5, (float)Math.Sin(Direction) * 5);

            // Hit
            if (level.GameObjects.Any(item => item.GetType().Name == "Enemy" && item.Box.Intersects(Box)))
            {
                Enemy enemy = (Enemy)level.GameObjects.First(item => item.GetType().Name == "Enemy" && item.Box.Intersects(Box));
                HitEnemy(enemy, level, this);
                Dead = true;
            }

            // Outside screen
            if (OutsideScreen())
            {
                Dead = true;
            }
        }
    }
}
