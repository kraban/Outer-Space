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
        public List<GameObject> SpaceObjects;
        private int delay;
        private int levelToBeSelected;
        public Text NewRank { get; set; }
        public Text NewItems { get; set; }
        private int playerDeath;
        public Enemy KilledPlayer { get; set; }

        // Boss
        private int numberOfJumps;
        private Vector2 bossPosition;
        private Vector2 bossLerpPosition;
        private int bossAnimationTimer;
        private int bossAnimationFrame;

        public Button Inventory { get; set; }
        public Button Menu { get; set; }
        public Button Rank { get; set; }

        public MapScene()
            : base()
        {
            this.Inventory = new Button(new Vector2(250, 30), "Inventory", TextureManager.SpriteFont20);
            this.Menu = new Button(new Vector2(0, Globals.ScreenSize.Y - 20), "Menu", TextureManager.SpriteFont20);
            this.Rank = new Button(new Vector2(600, 30), "Rank", TextureManager.SpriteFont20);
            this.SelectedLevel = -1;
            this.levelToBeSelected = -1;
            this.delay = -1;
            nearestLevels = new List<Level>();
            Initialize();

            this.NewRank = new Text(new Vector2(600, 70), "New Rank!", new Color(0, 255, 255), 0, 1f, true, TextureManager.SpriteFont15);
            this.NewItems = new Text(new Vector2(300, 70), "New Items!", new Color(0, 255, 255), 0, 1f, true, TextureManager.SpriteFont15);
        }

        public void Initialize()
        {
            this.SpaceObjects = new List<GameObject>();
            this.Levels = new List<Level>();
            this.ThePlayer = new Player();
            GenerateMap();
            this.playerDeath = 100;
            this.KilledPlayer = null;
            this.numberOfJumps = 0;
            bossPosition = new Vector2(-500, Globals.ScreenSize.Y / 2);
            bossLerpPosition = bossPosition;

            // Add modifiers
            // Add modifier positions not to close to each other
            List<Vector2> modifierPositions = new List<Vector2>();
            modifierPositions.Add(new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100)));
            for (int i = 1; i < Globals.Randomizer.Next(3, 5); i++)
            {
                Vector2 modifierPosition = new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100));
                while (modifierPositions.Any(item => Globals.Distance(item, modifierPosition) < 250) || Levels.Any(item => item.Distance(modifierPosition) < 50) || !Levels.Any(item => item.Distance(modifierPosition) < 100))
                {
                    modifierPosition = new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100));
                }
                modifierPositions.Add(modifierPosition);
            }

            // Add the modifiers
            for (int i = 0; i < modifierPositions.Count(); i++)
            {
                Modifier randomModifier = (Modifier)Enum.GetValues(typeof(Modifier)).GetValue(Globals.Randomizer.Next(1, Enum.GetValues(typeof(Modifier)).Length));
                if (randomModifier == Modifier.Sun)
                {
                    SpaceObjects.Add(new SpaceObject(TextureManager.sun, modifierPositions[i], 1, randomModifier));

                    foreach (Level level in Levels.Where(item => Globals.Distance(item.EnterLevel.Position, modifierPositions[i]) < 100))
                    {
                        level.LevelModifier = randomModifier;
                    }
                }
                else if (randomModifier == Modifier.Asteroid)
                {
                    Level level = ClosestLevels(modifierPositions[i])[0];
                    for (int j = 0; j < 4; j++)
                    {
                        level.LevelModifier = Modifier.Asteroid;
                        for (int k = 0; k < 3; k++)
                        {
                            SpaceObjects.Add(new SpaceObject(TextureManager.rock, new Vector2(level.EnterLevel.Position.X + Globals.Randomizer.Next(-30, 30), level.EnterLevel.Position.Y + Globals.Randomizer.Next(-30, 30)), 0.3f, randomModifier));
                        }
                        level = ClosestLevels(level.EnterLevel.Position).First(item => item.LevelModifier != Modifier.Asteroid);
                    }
                }
                else if (randomModifier == Modifier.BlackHole)
                {
                    SpaceObjects.Add(new SpaceObject(TextureManager.blackHole, modifierPositions[i], 1, randomModifier));

                    foreach (Level level in Levels.Where(item => Globals.Distance(item.EnterLevel.Position, modifierPositions[i]) < 100))
                    {
                        level.LevelModifier = randomModifier;
                    }
                }
                else if (randomModifier == Modifier.Satellite)
                {
                    SpaceObjects.Add(new SpaceObject(TextureManager.satellite, modifierPositions[i], 1, randomModifier));
                }
            }
        }

        public void GenerateMap()
        {
            while (true)
            {
                Levels.Clear();
                // Generate levels
                Levels.Add(new Level(new Vector2(50, Globals.ScreenSize.Y / 2), Difficulty.Easy));
                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100));
                    Levels.Add(new Level(position, position.X < Globals.ScreenSize.X / 3 ? Difficulty.Easy : position.X < Globals.ScreenSize.X * (2f/3f) ? Difficulty.Medium : Difficulty.Hard));
                }

                // Remove overlapping levels
                for (int i = 0; i < Levels.Count(); i++)
                {
                    for (int j = Levels.Count() - 1; j >= 1; j--)
                    {
                        if (Levels[i] != Levels[j] && Levels[i].Distance(Levels[j].EnterLevel.Position) < 20)
                        {
                            Levels.RemoveAt(j);
                        }
                    }
                }

                // Check if possible map
                List<Level> reachable = new List<Level>();
                reachable.Add(Levels[0]);
                for (int i = 0; i < reachable.Count(); i++)
                {
                    foreach (Level level in Levels.Where(item => !reachable.Any(item2 => item == item2) && item.Distance(reachable[i].EnterLevel.Position) < 150))
                    {
                        reachable.Add(level);
                    }
                }

                if (reachable.Count() / Levels.Count() > 0.9f)
                {
                    // Map succeded test, set first level to player startposition
                    Levels[0].PlayerOnStar = true;
                    Levels[0].Complete = true;
                    Levels[0].PlayerPosition = new Vector2(Levels[0].EnterLevel.Position.X + (float)Math.Cos(Levels[0].PlayerDirection) * 20, Levels[0].EnterLevel.Position.Y + (float)Math.Sin(Levels[0].PlayerDirection) * 20);
                    playerPosition = 0;
                    nearestLevels = FindNearestLevels(Levels[playerPosition]);
                    Levels.Add(new Level(Vector2.Zero, Difficulty.Boss));
                    break;
                }
                else
                {
                    continue;
                }
            }
        }

        // Move player from one star to another
        public void PlayerMove(int toLevel)
        {
            Levels[playerPosition].PlayerOnStar = false;
            Levels[toLevel].PlayerOnStar = true;
            Levels[toLevel].PlayerDirection = (float)Math.Atan2(Levels[toLevel].EnterLevel.Position.Y - Levels[playerPosition].PlayerPosition.Y, Levels[toLevel].EnterLevel.Position.X - Levels[playerPosition].PlayerPosition.X);
            Levels[toLevel].PlayerPosition = Levels[playerPosition].PlayerPosition;
            nearestLevels = FindNearestLevels(Levels[toLevel]);
            playerPosition = toLevel;
        }
        
        // Find all levels within 150 pixels to a specific level
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
            return nearestLevels;
        }

        public List<Level> ClosestLevels(Vector2 position)
        {
            List<Level> distances = new List<Level>();
            for (int i = 0; i < Levels.Count() - 1; i++)
            {
                distances.Add(Levels[i]);
            }
            distances = distances.OrderBy(item => item.Distance(position)).ToList();
            return distances;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (SelectedLevel == -1)
            {
                // Space objects
                foreach (GameObject spaceObject in SpaceObjects)
                {
                    spaceObject.Draw(spriteBatch);
                }

                if (NewItems.Flash > 0)
                {
                    NewItems.Flash++;
                    NewItems.Draw(spriteBatch);
                }

                if (NewRank.Flash > 0)
                {
                    NewRank.Flash++;
                    NewRank.Draw(spriteBatch);
                }


                Inventory.Draw(spriteBatch);
                Menu.Draw(spriteBatch);
                Rank.Draw(spriteBatch);

                // Boss
                spriteBatch.Draw(TextureManager.boss, bossPosition, null, Color.White, 0f, new Vector2(TextureManager.boss.Width / 2, TextureManager.boss.Height / 2), 0.6f, SpriteEffects.None, 0.2f);
                spriteBatch.Draw(TextureManager.bossEngineAnimation, bossPosition, new Rectangle(163 * bossAnimationFrame, 0, 163, 139), Color.White, 0f, new Vector2(TextureManager.boss.Width / 2, TextureManager.boss.Height / 2), 0.6f, SpriteEffects.None, 0.1f);
                for (int i = 0; i < 1000; i++)
                {
                    spriteBatch.Draw(TextureManager.pixel, new Rectangle((int)(bossPosition.X + Math.Cos(((float)i / 1000f) * Math.PI * 2) * 300), (int)(bossPosition.Y + Math.Sin(((float)i / 1000f) * Math.PI * 2) * 300), 3, 3), Color.Red);
                }

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

                for (int i = 0; i < Levels.Count() - 1; i++)
                {
                    Levels[i].DrawMap(spriteBatch);
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
                // Delay after klick on level to entering it
                delay--;
                if (delay == 0)
                {
                    SelectedLevel = levelToBeSelected;
                    levelToBeSelected = -1;
                    Levels[SelectedLevel].Initialize(ThePlayer);
                    numberOfJumps++;
                    bossLerpPosition += new Vector2(100, 0);
                }

                // Boss
                bossPosition = new Vector2(MathHelper.Lerp(bossPosition.X, bossLerpPosition.X, 0.01f), bossPosition.Y);
                bossAnimationTimer++;
                if (bossAnimationTimer > 5)
                {
                    bossAnimationTimer = 0;
                    if (bossAnimationFrame < 3)
                    {
                        bossAnimationFrame++;
                    }
                    else
                    {
                        bossAnimationFrame = 0;
                    }
                }

                // SpaceObjects
                foreach (GameObject spaceObject in SpaceObjects)
                {
                    spaceObject.Update();
                }

                for (int i = SpaceObjects.Count() - 1; i >= 0; i--)
                {
                    if (SpaceObjects[i].Dead)
                    {
                        SpaceObjects.RemoveAt(i);
                    }
                }

                if (!ThePlayer.Dead)
                {
                    NewItems.Update();
                    NewRank.Update();

                    // Boss
                    if (Globals.Distance(bossPosition, Levels[playerPosition].EnterLevel.Position) < 300 && delay < 0)
                    {
                        levelToBeSelected = Levels.Count() - 1;
                        delay = 65;
                    }

                    Inventory.Update();
                    if (Inventory.Press())
                    {
                        SceneManager.ChangeScene(SceneManager.inventoryScene);
                        NewItems.Flash = -1;
                    }

                    Menu.Update();
                    if (Menu.Press())
                    {
                        SceneManager.ChangeScene(SceneManager.menuScene);
                    }

                    Rank.Update();
                    if (Rank.Press())
                    {
                        SceneManager.ChangeScene(SceneManager.rankScene);
                        NewRank.Flash = -1;
                    }
                }
                else
                {
                    playerDeath--;
                    if (playerDeath == 0)
                    {
                        SceneManager.ChangeScene(SceneManager.gameOverScene);
                    }
                }

                for (int i = 0; i < Levels.Count() - 1; i++)
                {
                    Levels[i].UpdateMap();

                    if (!ThePlayer.Dead)
                    {
                        if (nearestLevels.Any(item => item == Levels[i]) && Levels[i].EnterLevel.Pressed() && delay < 0)
                        {
                            if (!Levels[i].Complete)
                            {
                                levelToBeSelected = i;
                                delay = 65;
                            }
                            PlayerMove(i);
                        }
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
