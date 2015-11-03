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
    public class MapScene : Scene
    {
        List<Level> Levels { get; set; }

        public MapScene()
            : base()
        {
            this.Levels = new List<Level>();
            for (int i = 0; i < 5; i++)
            {
                Levels.Add(new Level(new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100))));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (Level l in Levels)
            {
                spriteBatch.Draw(TextureManager.level, l.PositionOnMap, Color.White);
            }
        }
    }
}
