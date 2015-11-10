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
        public List<Level> Levels { get; set; }
        public Player ThePlayer { get; set; }
        public int SelectedLevel { get; set; }
        public Level CurrentLevel { get { return Levels[SelectedLevel]; } set { Levels[SelectedLevel] = value; } }

        public MapScene()
            : base()
        {
            this.Levels = new List<Level>();
            this.ThePlayer = new Player();
            this.SelectedLevel = -1;
            for (int i = 0; i < 5; i++)
            {
                Levels.Add(new Level(new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100))));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (SelectedLevel == -1)
            {
                foreach (Level l in Levels)
                {
                    l.EnterLevel.Draw(spriteBatch);
                }
            }
            else
            {
                Levels[SelectedLevel].Draw(spriteBatch);
            }
        }

        public override void Update()
        {
            base.Update();

            if (SelectedLevel == -1)
            {
                for (int i = 0; i < Levels.Count; i++)
			    {
                    Levels[i].EnterLevel.Update();

                    if (Levels[i].EnterLevel.Pressed() && !Levels[i].Compelete)
                    {
                        SelectedLevel = i;
                        Levels[i].Initialize(ThePlayer);
                    }
                }
            }
            else
            {
                Levels[SelectedLevel].Update();
            }
        }
    }
}
