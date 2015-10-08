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
    class Camera
    {
        public static Vector3 Position { get; set; }
        public static Vector3 LerpPosition { get; set; }
        public static int ScreenShakeTimer { get; set; }

        public static int ChangeSceneDelay { get; set; }

        public static void Update()
        {
            // Screen shake
            if (ScreenShakeTimer > 0)
            {
                ScreenShakeTimer--;
                int screenOffset = (int)MathHelper.Clamp(ScreenShakeTimer, 0, 10);
                Position = new Vector3(Globals.Randomizer.Next(-screenOffset, screenOffset), Globals.Randomizer.Next(-screenOffset, screenOffset), 0);
            }

            // Change scene
            ChangeSceneDelay--;
            if (ChangeSceneDelay == 0)
            {
                Camera.LerpPosition = new Vector3(0, -Globals.ScreenSize.Y - 50, 0);
            }

            if (ScreenShakeTimer <= 0)
            {
                if (!(Position.X < LerpPosition.X + 5 && Position.X > LerpPosition.X - 5 && Position.Y < LerpPosition.Y + 5 && Position.Y > LerpPosition.Y - 5))
                {
                    Position = new Vector3(MathHelper.Lerp(Position.X, LerpPosition.X, 0.1f), MathHelper.Lerp(Position.Y, LerpPosition.Y, 0.1f), 0);
                }
                else
                {
                    LerpPosition = new Vector3(0, 0, 0);
                }
            }
        }
    }
}
