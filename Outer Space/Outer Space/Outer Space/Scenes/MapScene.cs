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
    // ATT GÖRA
    /* Ta bort spelare från gammal stjärna
     * Alla stjärnor måste ha minst en annan stjärna som den når
     */

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
            Levels.Add(new Level(new Vector2(20, Globals.ScreenSize.Y / 2)));
            Levels[0].PlayerOnStar = true;
            Levels[0].Compelete = true;

            for (int i = 0; i < 40; i++)
            {
                Levels.Add(new Level(new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100))));
            }

            // Remove overlapping levels
            for (int i = Levels.Count - 1; i >= 1; i--)
            {
                if (Levels.Any(item => item != Levels[i] && Globals.Distance(Levels[i].EnterLevel.Position, item.EnterLevel.Position) < 20))
                {
                    Levels.RemoveAt(i);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (SelectedLevel == -1)
            {
                foreach (Level level in Levels.Where(item => item.EnterLevel.HoverOver() || item.PlayerOnStar))
                {
                    foreach (Level nextTo in Levels.Where(item => item != level && Globals.Distance(level.EnterLevel.Position, item.EnterLevel.Position) < 150))
                    {
                        Vector2 linePosition = nextTo.EnterLevel.Position - level.EnterLevel.Position;
                        float distance = Globals.Distance(level.EnterLevel.Position, nextTo.EnterLevel.Position) / 4;
                        for (float i = 0; i < distance; i++)
                        {
                            spriteBatch.Draw(TextureManager.pixel, new Rectangle((int)(level.EnterLevel.Position.X + linePosition.X * (i / distance)), (int)(level.EnterLevel.Position.Y + linePosition.Y * (i / distance)), 1, 1), Color.White);
                        }
                    }
                }

                foreach (Level l in Levels)
                {
                    l.DrawMap(spriteBatch);
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
                    Levels[i].UpdateMap();

                    if (!Levels[i].Compelete && Levels[i].EnterLevel.Pressed())
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
