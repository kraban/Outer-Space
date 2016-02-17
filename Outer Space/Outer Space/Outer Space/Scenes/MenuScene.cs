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
    public class MenuScene : Scene
    {
        public Button Start { get; set; }
        public Button Options { get; set; }
        public Button Tutorial { get; set; }
        public Button Quit { get; set; }
        public Button Continue { get; set; }

        public float Score { get; set; }
        public float LerpScore { get; set; }
        private float titleSize;
        private bool titleSizeFlip;

        public List<GameObject> GameObjects { get; set; }

        public MenuScene()
        {
            this.Start = new Button(new Vector2(200, 200), "New Game", TextureManager.SpriteFont20);
            this.Continue = new Button(new Vector2(200, 250), "Continue", TextureManager.SpriteFont20);
            this.Options = new Button(new Vector2(200, 300), "Options", TextureManager.SpriteFont20);
            this.Tutorial = new Button(new Vector2(200, 350), "Tutorial", TextureManager.SpriteFont20);
            this.Quit = new Button(new Vector2(200, 400), "Quit", TextureManager.SpriteFont20);

            this.GameObjects = new List<GameObject>();

            this.titleSize = 1;
        }

        public override void Update()
        {
            // Title
            if (titleSizeFlip)
            {
                if (titleSize > 1f)
                {
                    titleSize -= 0.001f;
                }
                else
                {
                    titleSize = MathHelper.Lerp(titleSize, 0.9f, 0.015f);
                }
                if (titleSize < 0.91f)
                {
                    titleSizeFlip = !titleSizeFlip;
                }
            }
            else
            {
                if (titleSize < 1f)
                {
                    titleSize += 0.001f;
                }
                else
                {
                    titleSize = MathHelper.Lerp(titleSize, 1.1f, 0.015f);
                }
                if (titleSize > 1.09f)
                {
                    titleSizeFlip = !titleSizeFlip;
                }
            }

            Start.Update();
            if (Start.Press())
            {
                SceneManager.ChangeScene(SceneManager.shipSelectScene);
                SceneManager.mapScene.NewItems.Flash = -1;
                SceneManager.mapScene.NewRank.Flash = -1;
            }

            if (SceneManager.started)
            {
                Continue.Update();
                if (Continue.Press())
                {
                    SceneManager.ChangeScene(SceneManager.mapScene);
                }
            }

            Options.Update();
            if (Options.Press())
            {
                SceneManager.ChangeScene(SceneManager.optionsScene);
            }

            Tutorial.Update();
            if (Tutorial.Press())
            {
                SceneManager.ChangeScene(SceneManager.tutorialScene);
            }

            Quit.Update();
            if (Quit.Press())
            {
                Game1.Quit();
            }

            // Rocks
            Score = MathHelper.Lerp(Score, LerpScore, 0.05f);

            if (Globals.Randomizer.Next(0, 101) < 2)
            {
                GameObjects.Add(new MenuRock());
            }

            foreach (GameObject go in GameObjects)
            {
                go.Update();
            }

            for (int i = GameObjects.Count - 1; i >= 0; i--)
            {
                if (GameObjects[i].Dead)
                {
                    if (GameObjects[i].GetType().Name == "MenuRock")
                    {
                        MenuRock rock = (MenuRock)GameObjects[i];
                        if (rock.MouseKill)
                        {
                            LerpScore += 100;
                            for (int j = 0; j < Globals.Randomizer.Next(5, 8); j++)
                            {
                                GameObjects.Add(new Piece(GameObjects[i].Position, GameObjects[i].Texture, 60, 1)); 
                            }
                        }
                    }
                    GameObjects.RemoveAt(i);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Start.Draw(spriteBatch);
            Options.Draw(spriteBatch);
            Continue.Draw(spriteBatch);
            Tutorial.Draw(spriteBatch);
            Quit.Draw(spriteBatch);

            // Warning
            if (Start.Hover() && SceneManager.started)
            {
                spriteBatch.DrawString(TextureManager.SpriteFont15, "Warning!\nIf you start a new game you will lose all previous progress!", new Vector2(430, 170), Color.Red);
            }

            foreach (GameObject go in GameObjects)
            {
                go.Draw(spriteBatch);
            }

            spriteBatch.DrawString(TextureManager.SpriteFont15, "Score: " + Score.ToString("0"), new Vector2(Globals.ScreenSize.X - 150, 0), Color.Yellow);
            spriteBatch.DrawString(TextureManager.SpriteFont50, "Outer Space", new Vector2(Globals.ScreenSize.X / 2, 10), new Color(0, 255, 255), 0f, new Vector2(TextureManager.SpriteFont50.MeasureString("Outer Space").X / 2, 0), titleSize, SpriteEffects.None, 0f);
        }
    }
}
