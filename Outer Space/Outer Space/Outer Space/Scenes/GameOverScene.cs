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
    public class GameOverScene : Scene
    {
        public Button Quit { get; set; }
        public Button Menu { get; set; }

        public GameOverScene()
        {
            this.Quit = new Button(new Vector2(200, 200), "Quit", TextureManager.SpriteFont20);
            this.Menu = new Button(new Vector2(200, 250), "Menu", TextureManager.SpriteFont20);
        }

        public override void Update()
        {
            base.Update();
            SceneManager.mapScene.KilledPlayer.UpdateGameOver();

            Quit.Update();
            if (Quit.Press())
	        {
                Game1.Quit();
	        }

            Menu.Update();
            if (Menu.Press())
            {
                SceneManager.ChangeScene(SceneManager.menuScene);
                SceneManager.started = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.DrawString(TextureManager.SpriteFont20, "Game Over", new Vector2(400, 60), Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            SceneManager.mapScene.KilledPlayer.DrawGameOver(spriteBatch);
            SceneManager.mapScene.KilledPlayer.Direction = SceneManager.mapScene.KilledPlayer.StandardDirection;

            Quit.Draw(spriteBatch);
            Menu.Draw(spriteBatch);
        }
    }
}
