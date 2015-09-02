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
    class Weapon : GameObject
    {
        // Public properties
        public int Damage { get; set; }
        public Shoot Action { get; set; }
        public List<Shoot> ShootMethods { get; set; }

        // Constructor(s)
        public Weapon()
            : base()
        {
            this.Damage = Globals.Randomizer.Next(10, 20);

            ShootMethods = new List<Shoot>();
            ShootMethods.Add(FireStandard);
            ShootMethods.Add(FireAiming);
            ShootMethods.Add(FireCrit);
            ShootMethods.Add(FireDelayEnemyShot);

            Action = ShootMethods[Globals.Randomizer.Next(0, ShootMethods.Count)];
        }

        // Method(s)

        public delegate void Shoot(Vector2 position, float direction, int tilesMatched, Level level);

        public void FireStandard(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitBasic));
        }

        public void FireAiming(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, (float)(Math.Atan2((level.GameObjects.First(item => item.GetType().Name == "Enemy").Position - position).Y, (level.GameObjects.First(item => item.GetType().Name == "Enemy").Position - position).X)), Damage, Shot.HitBasic));
        }

        public void FireCrit(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitCrit));
        }

        public void FireDelayEnemyShot(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitEnemyShotDelay));
        }
    }
}
