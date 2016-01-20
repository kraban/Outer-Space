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
        private int playerPosition;
        public Level CurrentLevel { get { return Levels[SelectedLevel]; } set { Levels[SelectedLevel] = value; } }
        private List<Level> nearestLevels;

        public MapScene()
            : base()
        {
            this.Levels = new List<Level>();
            this.ThePlayer = new Player();
            this.SelectedLevel = -1;
            Levels.Add(new Level(new Vector2(20, Globals.ScreenSize.Y / 2)));
            Levels[0].PlayerOnStar = true;
            Levels[0].Compelete = true;
            Levels[0].PlayerPosition = new Vector2(Levels[0].EnterLevel.Position.X + (float)Math.Cos(Levels[0].PlayerDirection) * 20, Levels[0].EnterLevel.Position.Y + (float)Math.Sin(Levels[0].PlayerDirection) * 20);
            playerPosition = 0;
            nearestLevels = new List<Level>();

            for (int i = 0; i < 40; i++)
            {
                Levels.Add(new Level(new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100))));
                // For test only, makes every level completed
                Levels[i].Compelete = true;
            }

            // Remove overlapping levels
            for (int i = Levels.Count - 1; i >= 1; i--)
            {
                if (Levels.Any(item => item != Levels[i] && Globals.Distance(Levels[i].EnterLevel.Position, item.EnterLevel.Position) < 20))
                {
                    Levels.RemoveAt(i);
                }
            }
            nearestLevels = FindNearestLevels(Levels[playerPosition]);
        }

        public void PlayerMove(int toLevel)
        {
            Levels[playerPosition].PlayerOnStar = false;
            Levels[toLevel].PlayerOnStar = true;
            Levels[toLevel].PlayerDirection = (float)Math.Atan2(Levels[toLevel].EnterLevel.Position.Y - Levels[playerPosition].PlayerPosition.Y, Levels[toLevel].EnterLevel.Position.X - Levels[playerPosition].PlayerPosition.X);
            Levels[toLevel].PlayerPosition = Levels[playerPosition].PlayerPosition;
            nearestLevels = FindNearestLevels(Levels[toLevel]);
            playerPosition = toLevel;
        }

        public List<Level> FindNearestLevels(Level startingLevel)
        {
            List<Level> nearestLevels = new List<Level>();
            if (Levels.Any(item => item != startingLevel && Globals.Distance(startingLevel.EnterLevel.Position, item.EnterLevel.Position) < 150))
            {
                foreach (Level nextTo in Levels.Where(item => item != startingLevel && Globals.Distance(startingLevel.EnterLevel.Position, item.EnterLevel.Position) < 150))
                {
                    nearestLevels.Add(nextTo);
                }
            }
            else
            {
                // If no star within range, search for the two closest
                List<Level> distances = new List<Level>();
                foreach (Level level in Levels)
                {
                    distances.Add(level);
                }
                distances = distances.OrderBy(item => Globals.Distance(item.EnterLevel.Position, Levels[playerPosition].EnterLevel.Position)).ToList();
                nearestLevels.Add(distances[1]);
                nearestLevels.Add(distances[2]);
            }
            return nearestLevels;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (SelectedLevel == -1)
            {
                // Draw lines to close stars
                foreach (Level level in nearestLevels)
                {
                    Vector2 linePosition = level.EnterLevel.Position - Levels[playerPosition].EnterLevel.Position;
                    float distance = Globals.Distance(Levels[playerPosition].EnterLevel.Position, level.EnterLevel.Position) / 4;
                    for (float i = 0; i < distance; i++)
                    {
                        spriteBatch.Draw(TextureManager.pixel, new Rectangle((int)(Levels[playerPosition].EnterLevel.Position.X + linePosition.X * (i / distance)), (int)(Levels[playerPosition].EnterLevel.Position.Y + linePosition.Y * (i / distance)), 1, 1), Color.White);
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

                    if (nearestLevels.Any(item => item == Levels[i]) && Levels[i].EnterLevel.Pressed())
                    {
                        if (!Levels[i].Compelete)
                        {
                            SelectedLevel = i;
                            Levels[i].Initialize(ThePlayer);
                        }
                        PlayerMove(i);
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
