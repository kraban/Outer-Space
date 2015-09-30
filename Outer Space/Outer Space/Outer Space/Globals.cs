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

        public static Vector3 ScreenShake { get; set; }
        public static int ScreenShakeTimer { get; set; }

        // Mouse
        public static MouseState MState { get; set; }
        public static MouseState PrevMState { get; set; }
        public static Rectangle MRectangle { get { return new Rectangle((int)MState.X, (int)MState.Y, 1, 1); } }

        // Keyboard
        public static KeyboardState KState { get; set; }
        public static KeyboardState PrevKState { get; set; }

        // Initialize
        public static void Initialize()
        {
            Randomizer = new Random();
        }

        // Update
        public static void Update()
        {
            // Screen shake
            if (ScreenShakeTimer > 0)
            {
                ScreenShakeTimer--;
                int screenOffset = (int)MathHelper.Clamp(ScreenShakeTimer, 0, 10);
                ScreenShake = new Vector3(Globals.Randomizer.Next(-screenOffset, screenOffset), Globals.Randomizer.Next(-screenOffset, screenOffset), 0);
            }

            PrevMState = MState;
            MState = Mouse.GetState();

            PrevKState = KState;
            KState = Keyboard.GetState();
        }
    }
}
