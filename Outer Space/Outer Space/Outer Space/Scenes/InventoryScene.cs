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
    public class InventoryScene : Scene
    {
        public Button Back { get; set; }

        public InventoryScene()
        {
            this.Back = new Button(new Vector2(Globals.ScreenSize.X - 200, 32), "Back", TextureManager.SpriteFont20);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);

            SceneManager.mapScene.ThePlayer.DrawInventory(spriteBatch);
        }

        public override void Update()
        {
            base.Update();

            SceneManager.mapScene.ThePlayer.UpdateInventory();

            Back.Update();
            if (Back.Press())
            {
                SceneManager.ChangeScene(SceneManager.mapScene);
            }
        }
    }
}
