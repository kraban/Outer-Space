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
        public Button Quit { get; set; }

        public Button Inventory { get; set; }

        public float Score { get; set; }
        public float LerpScore { get; set; }

        public List<GameObject> GameObjects { get; set; }

        public MenuScene()
        {
            this.Start = new Button(new Vector2(200, 200), "Start", TextureManager.SpriteFont20);
            this.Options = new Button(new Vector2(200, 250), "Options", TextureManager.SpriteFont20);
            this.Quit = new Button(new Vector2(200, 300), "Quit", TextureManager.SpriteFont20);

            this.Inventory = new Button(new Vector2(200, 400), "Inventory", TextureManager.SpriteFont20);

            this.GameObjects = new List<GameObject>();
        }

        public override void Update()
        {
            Start.Update();
            Options.Update();
            Quit.Update();

            Inventory.Update();
            if (Inventory.Press())
            {
                SceneManager.ChangeScene(SceneManager.inventoryScene);
            }

            if (Start.Press())
            {
                SceneManager.ChangeScene(SceneManager.shipSelectScene);
            }

            if (Options.Press())
            {
                SceneManager.ChangeScene(SceneManager.optionsScene);
            }

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
            Quit.Draw(spriteBatch);

            Inventory.Draw(spriteBatch);

            foreach (GameObject go in GameObjects)
            {
                go.Draw(spriteBatch);
            }

            spriteBatch.DrawString(TextureManager.SpriteFont15, "Score: " + Score.ToString("0"), new Vector2(Globals.ScreenSize.X - 150, 0), Color.Yellow);
        }
    }
}
