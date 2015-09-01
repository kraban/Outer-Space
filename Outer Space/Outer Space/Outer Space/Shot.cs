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

        // Constructor(s)
        public Shot(Vector2 position, float direction, int damage)
        {
            this.Position = position;
            this.Direction = direction;
            this.Damage = damage;
            this.Texture = TextureManager.shot;
        }

        // Method(s)
        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move
            Position += new Vector2((float)Math.Cos(Direction) * 5, (float)Math.Sin(Direction) * 5);

            // Hit
            if (level.GameObjects.Any(item => item.GetType().Name == "Enemy" && item.Box.Intersects(Box)))
            {
                Enemy enemy = (Enemy)level.GameObjects.First(item => item.GetType().Name == "Enemy" && item.Box.Intersects(Box));
                enemy.Health.Change(-Damage);
                Dead = true;
            }
        }
    }
}
