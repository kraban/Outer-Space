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
    public class ControlScene : Scene
    {
        public Button Back { get; set; }

        public ControlScene()
            : base()
        {
            Back = new Button(new Vector2(200, 200), "Back", TextureManager.SpriteFont20);
        }

        public override void Update()
        {
            base.Update();

            Back.Update();
            if (Back.Press())
            {
                SceneManager.ChangeScene(SceneManager.optionsScene);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);

            spriteBatch.DrawString(TextureManager.SpriteFont20, "Controls", new Vector2(Globals.ScreenSize.X / 2, 20), new Color(0, 255, 255), 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Move Left: A", new Vector2(Globals.ScreenSize.X / 2, 100), Color.White);
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Move Right: D", new Vector2(Globals.ScreenSize.X / 2, 150), Color.White);
        }
    }
}
