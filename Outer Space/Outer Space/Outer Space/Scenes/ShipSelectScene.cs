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

        public List<TextureButton> ShipButtons { get; set; }

        public ShipSelectScene()
            : base()
        {
            Back = new Button(new Vector2(200, 200), "Back", TextureManager.SpriteFont20);

            // Ship buttons
            ShipButtons = new List<TextureButton>();
            ShipButtons.Add(new TextureButton(new Vector2(Globals.ScreenSize.X / 2 - 200, Globals.ScreenSize.Y / 2), TextureManager.player));
            ShipButtons.Add(new TextureButton(new Vector2(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y / 2), TextureManager.ship2));
            ShipButtons.Add(new TextureButton(new Vector2(Globals.ScreenSize.X / 2 + 200, Globals.ScreenSize.Y / 2), TextureManager.player));
        }

        public override void Update()
        {
            base.Update();

            if (Back.Press())
            {
                SceneManager.ChangeScene(SceneManager.menuScene);
            }

            Back.Update();

            foreach (TextureButton shipButton in ShipButtons)
            {
                shipButton.Update();
                if (shipButton.Pressed())
                {
                    Globals.player.Texture = shipButton.Texture;
                    SceneManager.ChangeScene(SceneManager.mapScene);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);

            foreach (TextureButton shipButton in ShipButtons)
            {
                shipButton.Draw(spriteBatch);
            }
        }
    }
}
