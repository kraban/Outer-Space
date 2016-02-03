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
            ShipButtons.Add(new TextureButton(new Vector2(Globals.ScreenSize.X / 2 - 200, Globals.ScreenSize.Y / 2), TextureManager.ship1));
            ShipButtons.Add(new TextureButton(new Vector2(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y / 2), TextureManager.ship2));
            ShipButtons.Add(new TextureButton(new Vector2(Globals.ScreenSize.X / 2 + 200, Globals.ScreenSize.Y / 2), TextureManager.ship3));
        }

        public override void Update()
        {
            base.Update();

            if (Back.Press())
            {
                SceneManager.ChangeScene(SceneManager.menuScene);
            }

            Back.Update();

            for (int i = 0; i < ShipButtons.Count(); i++)
			{
                ShipButtons[i].Update();
                if (ShipButtons[i].Pressed())
                {
                    SceneManager.ChangeScene(SceneManager.mapScene);
                    SceneManager.mapScene.ThePlayer.Texture = ShipButtons[i].Texture;
                    if (i == 0)
                    {
                        SceneManager.mapScene.ThePlayer.engineAnimation = TextureManager.ship1EngineAnimation;
                        SceneManager.mapScene.ThePlayer.maxFrame = 3;
                        break;
                    }
                    else if (i == 1)
                    {
                        SceneManager.mapScene.ThePlayer.engineAnimation = TextureManager.ship2EngineAnimation;
                        SceneManager.mapScene.ThePlayer.maxFrame = 3;
                        break;
                    }
                    else if (i == 2)
                    {
                        SceneManager.mapScene.ThePlayer.engineAnimation = TextureManager.ship3EngineAnimation;
                        SceneManager.mapScene.ThePlayer.maxFrame = 1;
                        break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);

            spriteBatch.DrawString(TextureManager.SpriteFont20, "Choose a ship", new Vector2(Globals.ScreenSize.X / 2 - TextureManager.SpriteFont20.MeasureString("Choose a ship").X * (1.5f / 2f), 200), new Color(0, 255, 255), 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

            foreach (TextureButton shipButton in ShipButtons)
            {
                shipButton.Draw(spriteBatch);
            }
        }
    }
}
