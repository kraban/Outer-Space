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
    public class Weapon : GameObject
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
            ShootMethods.Add(FireStandardPlayer);
            ShootMethods.Add(FireAimingPlayer);
            ShootMethods.Add(FireCritPlayer);
            ShootMethods.Add(FireDelayEnemyShotPlayer); 

            Action = ShootMethods[Globals.Randomizer.Next(0, ShootMethods.Count)];
        }

        // Method(s)

        public delegate void Shoot(Vector2 position, float direction, int tilesMatched, Level level);


        public void FireStandardPlayer(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new PlayerShot(position, direction, Damage, PlayerShot.HitBasic));
        }

        public void FireAimingPlayer(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new PlayerShot(position, (float)(Math.Atan2((level.GameObjects.First(item => item.GetType().Name == "Enemy").Position - position).Y, (level.GameObjects.First(item => item.GetType().Name == "Enemy").Position - position).X)), Damage, PlayerShot.HitBasic));
        }

        public void FireCritPlayer(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new PlayerShot(position, direction, Damage, PlayerShot.HitCrit));
        }

        public void FireDelayEnemyShotPlayer(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new PlayerShot(position, direction, Damage, PlayerShot.HitEnemyShotDelay));
        }
    }
}
