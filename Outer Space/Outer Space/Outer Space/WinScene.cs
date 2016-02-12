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
    public class WinScene : Scene
    {
        public Button Quit { get; set; }
        public Button Menu { get; set; }
        private int timer;
        private Vector2 fireworksPosition;

        public WinScene()
            : base()
        {
            Menu = new Button(new Vector2(200, 200), "Menu", TextureManager.SpriteFont20);
            Quit = new Button(new Vector2(200, 250), "Quit", TextureManager.SpriteFont20);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.DrawString(TextureManager.SpriteFont20, "Victory!", new Vector2(Globals.ScreenSize.X / 2 - 70, 20), new Color(0, 255, 255), 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

            Menu.Draw(spriteBatch);
            Quit.Draw(spriteBatch);
        }
        public override void Update()
        {
            base.Update();

            Menu.Update();
            if (Menu.Press())
            {
                SceneManager.ChangeScene(SceneManager.menuScene);
                SceneManager.started = false;
            }

            Quit.Update();
            if (Quit.Press())
            {
                Game1.Quit();
            }

            if (Globals.Randomizer.Next(0, 101) < 3 && timer < 0)
            {
                timer = 15;
                fireworksPosition = new Vector2(Globals.Randomizer.Next(100, Globals.ScreenSize.X - 100), Globals.Randomizer.Next(100, Globals.ScreenSize.Y - 100));
            }

            timer--;
            if (timer > 0)
            {
                for (int i = 0; i < Globals.Randomizer.Next(30, 50); i++)
                {
                    SceneManager.GameObjects.Add(new Firework(fireworksPosition));
                }
            }
        }
    }
}
