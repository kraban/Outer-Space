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
        public List<String> Targets { get; set; }
        public int Disabled { get; set; }

        // Constructor(s)
        public Weapon()
            : base()
        {
            this.Damage = Globals.Randomizer.Next(10, 20);
            this.Depth = 0.5f;

            this.Texture = TextureManager.player;

            ShootMethods = new List<Shoot>();
            ShootMethods.Add(FireStandardPlayer);
            ShootMethods.Add(FireAimingPlayer);
            ShootMethods.Add(FireCritPlayer);
            ShootMethods.Add(FireDelayEnemyShotPlayer); 

            Action = ShootMethods[Globals.Randomizer.Next(0, ShootMethods.Count)];

            Targets = new List<string>();
        }

        // Method(s)
        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            if (Disabled >= 0)
            {
                Disabled--;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Description
            if (Globals.MRectangle.Intersects(Box))
            {
                spriteBatch.DrawString(TextureManager.SpriteFont15, "Damage: " + Damage, new Vector2(Position.X + Texture.Width / 2 + 20, Position.Y - Texture.Height / 2), Color.White);
            }

            // Disabled
            if (Disabled > 0)
            {
                spriteBatch.Draw(TextureManager.jammed, new Vector2(Position.X - Texture.Width / 2, Position.Y - Texture.Height / 2), Color.White);
            }
        }

        public delegate void Shoot(Vector2 position, float direction, int tilesMatched, Level level);


        public void FireStandardPlayer(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitBasic, Targets));
        }

        public void FireAimingPlayer(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, (float)(Math.Atan2((level.GameObjects.First(item => Targets.Any(target => target == item.GetType().Name)).Position - position).Y, (level.GameObjects.First(item => Targets.Any(target => target == item.GetType().Name)).Position - position).X)), Damage, Shot.HitBasic, Targets));
        }

        public void FireCritPlayer(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitCrit, Targets));
        }

        public void FireDelayEnemyShotPlayer(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitEnemyShotDelay, Targets));
        }
    }
}
