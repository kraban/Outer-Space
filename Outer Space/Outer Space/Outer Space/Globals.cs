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
    public enum DamageType { laser, rock, damageOverTime }

    class Globals
    {
        public static Random Randomizer { get; set; }

        public static Point ScreenSize { get { return new Point(1124, 600); } }
        public static Point CombatScreenSize { get { return new Point(ScreenSize.X / 2, ScreenSize.Y); } }

        // Mouse
        public static MouseState MState { get; set; }
        public static MouseState PrevMState { get; set; }
        public static Rectangle MRectangle { get { return new Rectangle((int)MState.X, (int)MState.Y, 1, 1); } }

        // Keyboard
        public static KeyboardState KState { get; set; }
        public static KeyboardState PrevKState { get; set; }

        // ItemTemplates
        public static Item Heal { get; set; }
        public static Item Flee { get; set; }
        public static Item Nothing { get; set; }

        // Possible enemymodules
        public static List<int> EnemyWeapons { get; set; }
        public static List<int> EnemyHulls { get; set; }
        public static List<int> EnemyShields { get; set; }

        // Initialize
        public static void Initialize()
        {
            Randomizer = new Random();

            Heal = new Item(Item.HealPlayer, ItemType.misc, TextureManager.wrench, "|W|Right click to regain 10 % health.", "Wrench");
            Flee = new Item(Item.Flee, ItemType.misc, TextureManager.flee, "|W|Used to flee from combat.", "Flee");
            Nothing = new Item(Item.Nothing, ItemType.nothing, TextureManager.none, "", "");

            // Possible enemymodules
            EnemyWeapons = new List<int>();
            for (int i = 0; i < Weapon.ListOfMethods().Count(); i++)
            {
                EnemyWeapons.Add(i);
            }
            EnemyWeapons.Remove(1);
            EnemyWeapons.Remove(8);
            EnemyWeapons.Remove(15);
            EnemyWeapons.Remove(18);

            EnemyHulls = new List<int>();
            EnemyHulls.Add(0);
            EnemyHulls.Add(2);
            EnemyHulls.Add(7);

            EnemyShields = new List<int>();
            EnemyShields.Add(0);
            EnemyShields.Add(1);
            EnemyShields.Add(2);
            EnemyShields.Add(4);
            EnemyShields.Add(5);
            EnemyShields.Add(7);
        }

        public static float Distance(Vector2 v1, Vector2 v2)
        {
            return (v2 - v1).Length();
        }

        // Update
        public static void Update()
        {
            PrevMState = MState;
            MState = Mouse.GetState();

            PrevKState = KState;
            KState = Keyboard.GetState();
        }
    }
}
