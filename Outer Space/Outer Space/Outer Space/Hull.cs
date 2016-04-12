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
        public List<string> Descriptions { get; set; }
        public HullMethod Method { get; set; }
        
        // Method variable(s)
        public float RockResistance { get; set; }
        public int WeaponChance { get; set; }
        public List<TileType> TileChance { get; set; }
        public bool FlashPossibleTiles { get; set; }
        public float WeaponAccuracy { get; set; }

        // Constructor(s)
        public Hull(Ship ship, int method, int itemLevel)
            : base(Item.Nothing, ItemType.hull, TextureManager.hulls[Globals.Randomizer.Next(0, TextureManager.hulls.Count)], "", "Hull")
        {
            this.Type = ItemType.hull;
            this.ItemLevel = itemLevel;

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

            if (itemLevel != -1)
            {
                this.Armor = Globals.Randomizer.Next(5 + itemLevel * 3, 10 + itemLevel * 4);
            }
            else
            {
                this.Armor = 7;
            }

            this.Method = ListOfHullMethods()[method];
            Method(ship, this);

            this.Descriptions = new List<string>();

            Descriptions.Add("A standard hull.");
            Descriptions.Add("Rocks deals half damage.");
            Descriptions.Add("Increase |255, 70, 0|weapon chance|W| by " + WeaponChance + "%");
            Descriptions.Add("Increase the chance of cog tiles appearing by 100%");
            Descriptions.Add("Increase the chance of shield tiles appearing by 100%");
            Descriptions.Add("Increase the chance of weapon tiles appearing by 75%");
            Descriptions.Add("Flash a random possible tilematch when there has been no match for a few seconds.");
            Descriptions.Add("Increases the accuracy of some weapons.");

            this.Description = "|W|Armor: |255,255,0|" + Armor + "|W|\n" + Descriptions[method];
        }

        // Method(s)
        public static List<HullMethod> ListOfHullMethods()
        {
            List<HullMethod> methods = new List<HullMethod>();
            methods.Add(HullStandard);
            methods.Add(HullRockResist);
            methods.Add(HullWeaponChance);
            methods.Add(HullTileCogChance);
            methods.Add(HullTileShieldChance);
            methods.Add(HullTileShootChance);
            methods.Add(HullFlashPossibleTiles);
            methods.Add(HullIncreaseWeaponAccuracy);
            return methods;
        }

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

        public delegate void HullMethod(Ship ship, Hull hull);

        public static void HullStandard(Ship ship, Hull hull)
        { }

        public static void HullRockResist(Ship ship, Hull hull)
        {
            hull.RockResistance = 0.5f;
        }

        public static void HullWeaponChance(Ship ship, Hull hull)
        {
            hull.WeaponChance = Globals.Randomizer.Next(10, 21);
        }

        public static void HullTileCogChance(Ship ship, Hull hull)
        {
            for (int i = 0; i < 20; i++)
            {
                hull.TileChance.Add(TileType.cog);
            }
        }

        public static void HullTileShieldChance(Ship ship, Hull hull)
        {
            for (int i = 0; i < 20; i++)
            {
                hull.TileChance.Add(TileType.shield);
            }
        }

        public static void HullTileShootChance(Ship ship, Hull hull)
        {
            for (int i = 0; i < 15; i++)
            {
                hull.TileChance.Add(TileType.shoot);
            }
        }

        public static void HullFlashPossibleTiles(Ship ship, Hull hull)
        {
            hull.FlashPossibleTiles = true;
        }

        public static void HullIncreaseWeaponAccuracy(Ship ship, Hull hull)
        {
            hull.WeaponAccuracy += 0.25f;
        }
    }
}
