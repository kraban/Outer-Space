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
    class MenuRock : GameObject
    {
        public float Speed { get; set; }

        public MenuRock()
            : base()
        {
            this.Position = new Vector2(-30, Globals.Randomizer.Next(0, Globals.ScreenSize.Y));
            this.Texture = TextureManager.rock;
            this.Speed = MathHelper.Lerp(1, 4, (float)Globals.Randomizer.NextDouble());
            this.Depth = 1;
        }

        // Method(s)
        public override void Update()
        {
            base.Update();

            // Move
            Position += new Vector2(Speed, 0);

            // Hit
            if (Globals.MRectangle.Intersects(Box) && Globals.MState.LeftButton == ButtonState.Pressed && Globals.PrevMState.LeftButton == ButtonState.Released)
            {
                Dead = true;
            }
        }
    }
}
