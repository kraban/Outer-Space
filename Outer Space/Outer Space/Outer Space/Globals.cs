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
    class Globals
    {
        public static Random Randomizer { get; set; }

        public static Point ScreenSize { get { return new Point(1024, 600); } }

        // Mouse
        public static MouseState MState { get; set; }
        public static MouseState PrevMState { get; set; }
        public static Rectangle MRectangle { get { return new Rectangle((int)MState.X, (int)MState.Y, 1, 1); } }

        // Initialize
        public static void Initialize()
        {
            Randomizer = new Random();
        }

        // Update
        public static void Update()
        {
            PrevMState = MState;
            MState = Mouse.GetState();
        }
    }
}
