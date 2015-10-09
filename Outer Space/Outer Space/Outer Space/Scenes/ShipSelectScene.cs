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
    public class ShipSelectScene : Scene
    {
        public Button Back { get; set; }

        public ShipSelectScene()
            : base()
        {
            Back = new Button(new Vector2(200, 200), "Back", TextureManager.SpriteFont20);
        }

        public override void Update()
        {
            base.Update();

            if (Back.Press())
            {
                SceneManager.ChangeScene(SceneManager.menuScene);
            }

            Back.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);
        }
    }
}
