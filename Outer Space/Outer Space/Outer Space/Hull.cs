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
    public class Hull : Item
    {
        // Public properties
        public int Armor { get; set; }
        public List<HullMethod> HullMethods { get; set; }
        public List<string> Descriptions { get; set; }
        public int Method { get; set; }
        
        // Method variable(s)
        public float RockResistance { get; set; }
        public int WeaponChance { get; set; }
        public List<TileType> TileChance { get; set; }

        // Constructor(s)
        public Hull(Ship ship)
            : base()
        {
            this.Type = ItemType.hull;

            // Method variables
            RockResistance = 1;
            this.TileChance = new List<TileType>();
            for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    TileChance.Add((TileType)i);
                }
            }

            this.Texture = TextureManager.hulls[Globals.Randomizer.Next(0, TextureManager.hulls.Count)];

            this.Armor = Globals.Randomizer.Next(5, 10);

            this.HullMethods = new List<HullMethod>();
            HullMethods.Add(HullStandard);
            HullMethods.Add(HullRockResist);
            HullMethods.Add(HullWeaponChance);
            HullMethods.Add(HullTileCogChance);
            HullMethods.Add(HullTileShieldChance);
            HullMethods.Add(HullTileShootChance);

            this.Method = Globals.Randomizer.Next(0, HullMethods.Count);
            HullMethods[Method](ship);

            this.Descriptions = new List<string>();

            Descriptions.Add("A standard hull.");
            Descriptions.Add("Rocks deals half damage.");
            Descriptions.Add("Increase |255, 70, 0|weapon chance|W| by " + WeaponChance + "%");
            Descriptions.Add("Increase the chance of cog tiles appearing by 100%");
            Descriptions.Add("Increase the chance of shield tiles appearing by 100%");
            Descriptions.Add("Increase the chance of weapon tiles appearing by 100%");

            this.Description = "|W|Armor: |255,255,0|" + Armor + "|W|\n" + Descriptions[Method];
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
            if (Globals.MRectangle.Intersects(Box) && SceneManager.CurrentScene == SceneManager.mapScene)
            {
                Text.TextDifferentColor(spriteBatch, Description, new Vector2(Position.X + Texture.Width / 2 + 84, Position.Y - Texture.Height / 2), 1f, TextureManager.SpriteFont15, false);
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
                w.LoadDescriptions();
            }
        }

        public void HullTileCogChance(Ship ship)
        {
            for (int i = 0; i < 20; i++)
            {
                TileChance.Add(TileType.cog);
            }
        }

        public void HullTileShieldChance(Ship ship)
        {
            for (int i = 0; i < 20; i++)
            {
                TileChance.Add(TileType.shield);
            }
        }

        public void HullTileShootChance(Ship ship)
        {
            for (int i = 0; i < 20; i++)
            {
                TileChance.Add(TileType.shoot);
            }
        }
    }
}
