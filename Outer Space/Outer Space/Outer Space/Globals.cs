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

        // Initialize
        public static void Initialize()
        {
            Randomizer = new Random();

            Heal = new Item(Item.HealPlayer, ItemType.misc, TextureManager.wrench, "|W|Right click to regain 10 % health.", "Wrench");
            Flee = new Item(Item.Flee, ItemType.misc, TextureManager.flee, "|W|Used to flee from combat.", "Flee");
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
