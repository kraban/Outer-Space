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

        public MenuScene()
        {
            this.Start = new Button(new Vector2(200, 200), "Start", TextureManager.SpriteFont20);
            this.Options = new Button(new Vector2(200, 250), "Options", TextureManager.SpriteFont20);
            this.Quit = new Button(new Vector2(200, 300), "Quit", TextureManager.SpriteFont20);
        }

        public override void Update()
        {
            Start.Update();
            Options.Update();
            Quit.Update();

            if (Start.Press())
            {
                //SceneManager.ChangeMenu(SceneManager.
            }

            if (Options.Press())
            {
                SceneManager.ChangeScene(SceneManager.optionsScene);
            }

            if (Quit.Press())
            {
                Game1.Quit();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Start.Draw(spriteBatch);
            Options.Draw(spriteBatch);
            Quit.Draw(spriteBatch);
        }
    }
}
