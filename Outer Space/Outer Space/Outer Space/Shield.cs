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
    public class Shield : Item
    {
        // Public properties
        public Bar ShieldBar { get; set; }
        public List<string> Descriptions { get; set; }
        public List<ShieldMethod> ShieldMethods { get; set; }
        public int Method { get; set; }
        public bool Combat { get; set; }
        public float ShieldHeal { get; set; }

        // Shieldbar
        public float MaxValue { get { return ShieldBar.MaxValue; } set { ShieldBar.MaxValue = value; } }
        public float Value { get { return ShieldBar.Value; } }
        public float Width { get { return ShieldBar.Width; } }

        // Constructor(s)
        public Shield(Vector2 position, int width, int height, float shieldValue)
            : base()
        {
            this.Type = ItemType.shield;
            this.Texture = TextureManager.shields[Globals.Randomizer.Next(0, TextureManager.shields.Count)];
            this.ShieldHeal = Globals.Randomizer.Next(6, 14);

            this.ShieldBar = new Bar(position, width, height, shieldValue, Color.LightBlue);

            this.ShieldMethods = new List<ShieldMethod>();
            ShieldMethods.Add(ShieldStandard);
            ShieldMethods.Add(ShieldReflect);

            this.Method = Globals.Randomizer.Next(0, ShieldMethods.Count);

            this.Descriptions = new List<string>();
            Descriptions.Add("A standard shield.");
            Descriptions.Add("Has a 10% chance to reflect a shot.");

            this.Description = "|W|Shield: |0,0,255|" + MaxValue + "|W|\n" + Descriptions[Method];
        }

        // Method(s)
        public float Change(float value)
        {
            return ShieldBar.Change(value);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Combat)
            {
                ShieldBar.Draw(spriteBatch); 
            }

            // Description
            if (Globals.MRectangle.Intersects(Box) && Combat)
            {
                Text.TextDifferentColor(spriteBatch, Description, new Vector2(Position.X + Texture.Width / 2 + 84, Position.Y - Texture.Height / 2), 1f, TextureManager.SpriteFont15, false);
            }
        }

        public delegate void ShieldMethod(float damage, DamageType damageType, Ship ship);

        public void ShieldStandard(float damage, DamageType damageType, Ship ship)
        { }

        public void ShieldReflect(float damage, DamageType damageType, Ship ship)
        {
            if (damageType == DamageType.laser && Globals.Randomizer.Next(0, 10) < 1)
            {
                List<string> targets = new List<string>();
                targets.Add(ship.GetType().Name == "Player" ? "Enemy" : "Player");
                SceneManager.mapScene.CurrentLevel.ToAdd.Add(new Shot(ship.Position, MathHelper.Lerp(0, (float)Math.PI * 2, (float)Globals.Randomizer.NextDouble()), damage, Shot.HitBasic, targets, 0f, 0));
            }
        }
    }
}
