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
    public class Hull : GameObject
    {
        // Public properties
        public int Armor { get; set; }
        public List<HullMethod> HullMethods { get; set; }
        public List<string> Description { get; set; }
        public int Method { get; set; }
        
        // Method variable(s)
        public float RockResistance { get; set; }
        public int WeaponChance { get; set; }

        // Constructor(s)
        public Hull(Ship ship)
            : base()
        {
            // Method variables
            RockResistance = 1;

            this.Texture = TextureManager.hulls[Globals.Randomizer.Next(0, TextureManager.hulls.Count)];

            this.Armor = Globals.Randomizer.Next(5, 10);

            this.HullMethods = new List<HullMethod>();
            HullMethods.Add(HullStandard);
            HullMethods.Add(HullRockResist);
            HullMethods.Add(HullWeaponChance);

            this.Method = Globals.Randomizer.Next(0, HullMethods.Count);
            HullMethods[Method](ship);

            this.Description = new List<string>();

            Description.Add("A standard hull.");
            Description.Add("Rocks deals half damage.");
            Description.Add("Increase |255, 70, 0|weapon chance|W| by " + WeaponChance + "%");
        }

        // Method(s)
        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Description
            if (Globals.MRectangle.Intersects(Box))
            {
                Text.TextDifferentColor(spriteBatch, "|W|Armor: |255,255,0|" + Armor + "|W|\n" + Description[Method], new Vector2(Position.X + Texture.Width / 2 + 84, Position.Y - Texture.Height / 2), 1f, TextureManager.SpriteFont15, false);
            }
        }

        public delegate void HullMethod(Ship ship);

        public void HullStandard(Ship ship)
        { }

        public void HullRockResist(Ship ship)
        {
            RockResistance = 0.5f;
        }

        public void HullWeaponChance(Ship ship)
        {
            WeaponChance = Globals.Randomizer.Next(10, 21);
            foreach (Weapon w in ship.Weapons)
            {
                w.Chance += WeaponChance;
                w.LoadDescription();
            }
        }
    }
}
