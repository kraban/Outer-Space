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
    class EnemyShot : Shot
    {
        // Public properties
        public Hit HitPlayer { get; set; }

        // Constructor(s)
        public EnemyShot(Vector2 position, float direction, float damage, Hit hit)
            : base(position, direction, damage)
        {
            HitPlayer = hit;
        }

        // Method(s)
        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Hit
            if (level.Player.Box.Intersects(Box))
            {
                HitPlayer(level.Player, level, this);
                Dead = true;
            }
        }

        public delegate void Hit(Player player, Level level, Shot shot);

        public static void HitBasic(Player player, Level level, Shot shot)
        {
            player.TakeDamage(shot.Damage * 2.5f, 0f);
        }
    }
}
