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
        public float ShieldPiercing { get; set; }
        public int Action { get; set; }
        public List<Shoot> ShootMethods { get; set; }
        public List<String> Description { get; set; }
        public List<String> Targets { get; set; }
        public int Disabled { get; set; }

        // Private variables
        private bool drawDescription;
        private int drawDescriptionTimer;

        // Constructor(s)
        public Weapon()
            : base()
        {
            this.Damage = Globals.Randomizer.Next(10, 20);
            this.ShieldPiercing = (float)Math.Round(Globals.Randomizer.NextDouble(), 2);
            this.Depth = 0.5f;

            this.Texture = TextureManager.player;

            ShootMethods = new List<Shoot>();
            ShootMethods.Add(FireStandard);
            ShootMethods.Add(FireAiming);
            ShootMethods.Add(FireCrit);
            ShootMethods.Add(FireDelayEnemyShot);
            ShootMethods.Add(FireDamageOverTime);

            Action = Globals.Randomizer.Next(0, ShootMethods.Count);

            Targets = new List<string>();

            // Description
            Description = new List<string>();
            Description.Add("Shoot a standard shot");
            Description.Add("Shoot a shot that aims at a random target");
            Description.Add("Shoot a shot that has a chance to crit");
            Description.Add("Shoot a shot that has a chance to disable\n a random target weapon for a few seconds");
            Description.Add("Shoot a shot that deals damage over time");
        }

        // Method(s)
        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            if (Disabled >= 0)
            {
                Disabled--;
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
                spriteBatch.DrawString(TextureManager.SpriteFont15, "Damage: " + Damage + "\nShield Piercing: " + ShieldPiercing * 100 + "%\n" + Description[Action], new Vector2(Position.X + Texture.Width / 2 + 20, Position.Y - Texture.Height / 2), Color.White);
            }

            // Disabled
            if (Disabled > 0)
            {
                spriteBatch.Draw(TextureManager.jammed, new Vector2(Position.X - Texture.Width / 2, Position.Y - Texture.Height / 2), Color.White);
            }
        }

        public delegate void Shoot(Vector2 position, float direction, int tilesMatched, Level level);


        public void FireStandard(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitBasic, Targets, ShieldPiercing));
        }

        public void FireAiming(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, (float)(Math.Atan2((level.GameObjects.First(item => Targets.Any(target => target == item.GetType().Name)).Position - position).Y, (level.GameObjects.First(item => Targets.Any(target => target == item.GetType().Name)).Position - position).X)), Damage, Shot.HitBasic, Targets, ShieldPiercing));
        }

        public void FireCrit(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitCrit, Targets, ShieldPiercing));
        }

        public void FireDelayEnemyShot(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitEnemyShotDelay, Targets, ShieldPiercing));
        }

        public void FireDamageOverTime(Vector2 position, float direction, int tilesMatched, Level level)
        {
            level.ToAdd.Add(new Shot(position, direction, Damage, Shot.HitDamageOverTime, Targets, ShieldPiercing));
        }
    }
}
