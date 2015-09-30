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
    public class Shield : GameObject
    {
        // Public properties
        public Bar ShieldBar { get; set; }
        public List<string> Description { get; set; }
        public List<ShieldMethod> ShieldMethods { get; set; }
        public int Method { get; set; }

        // Shieldbar
        public float MaxValue { get { return ShieldBar.MaxValue; } set { ShieldBar.MaxValue = value; } }
        public float Value { get { return ShieldBar.Value; } }
        public float Width { get { return ShieldBar.Width; } }

        // Constructor(s)
        public Shield(Vector2 position, int width, int height, float shieldValue)
            : base()
        {
            this.Texture = TextureManager.shields[Globals.Randomizer.Next(0, TextureManager.shields.Count)];

            this.ShieldBar = new Bar(position, width, height, shieldValue, Color.LightBlue);

            this.ShieldMethods = new List<ShieldMethod>();
            ShieldMethods.Add(ShieldStandard);

            this.Method = Globals.Randomizer.Next(0, ShieldMethods.Count);

            this.Description = new List<string>();
            Description.Add("A standard shield.");
        }

        // Method(s)
        public float Change(float value)
        {
            return ShieldBar.Change(value);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            ShieldBar.Draw(spriteBatch);

            // Description
            if (Globals.MRectangle.Intersects(Box))
            {
                Text.TextDifferentColor(spriteBatch, "|W|Shield: |0,0,255|" + MaxValue + "|W|\n" + Description[Method], new Vector2(Position.X + Texture.Width / 2 + 84, Position.Y - Texture.Height / 2), 1f, TextureManager.SpriteFont15, false);
            }
        }

        public delegate void ShieldMethod();

        public void ShieldStandard()
        { }
    }
}
